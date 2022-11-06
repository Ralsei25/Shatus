using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using VkNet;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Utils.AntiCaptcha;

namespace Shatus.Vk;

public class ShatusVkPublisher : IDisposable
{
    private readonly VideoUploader _videoUploader;
    private readonly IOptions<VkConfigs> _options;
    private readonly ILogger<ShatusVkPublisher>? _logger;
    private IVkApi _api = new VkApi();
    private readonly SemaphoreSlim _semaphoreSlim = new(1);

	public ShatusVkPublisher(
        VideoUploader videoUploader, IOptions<VkConfigs> options, 
        ILogger<ShatusVkPublisher>? logger, ILogger<VkApi>? apiLogger, 
        ICaptchaSolver captchaSolver)
	{
        _videoUploader = videoUploader;
        _options = options;
        _logger = logger;
        _api = new VkApi(apiLogger, captchaSolver);
	}

	public async Task ScheduleVideo(string videoName, string filePath)
	{
        try
        {
            await _semaphoreSlim.WaitAsync();
            await AuthorizeAsync();
            var video = await SaveVideoAsync(videoName, filePath);
            var publishDate = await GetPublishDateAsync();
            await SchedulePostAsync(publishDate, video);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
	}

    private async Task SchedulePostAsync(DateTime publishDate, Video video)
    {
        await _api.Wall.PostAsync(new()
        {
            OwnerId = -_options.Value.GroupId,
            FromGroup = true,
            Signed = false,
            PublishDate = publishDate,
            Attachments = new MediaAttachment[] { video }
        });
    }
	private async Task<DateTime> GetPublishDateAsync()
    {
        var publishDate = DateTime.Now.AddHours(1);
        var wall = await _api.Wall.GetAsync(new()
        {
            OwnerId = -_options.Value.GroupId,
            Filter = WallFilter.Postponed
        });

        var lastPost = wall.WallPosts
            .Where(p => p.Date.Value.Date == DateTime.Today)
            .OrderByDescending(p => p.Date)
            .FirstOrDefault();

        if (lastPost is not null)
        {
            publishDate = lastPost.Date.Value.AddHours(4);
        }
        return publishDate;
    }
    private async Task<Video> SaveVideoAsync(string videoName, string filePath)
    {
        var video = await _api.Video.SaveAsync(new()
        {
            Name = videoName,
            Description = videoName,
            GroupId = _options.Value.GroupId,
        });
        await _videoUploader.UploadVideoAsync(video.UploadUrl.AbsoluteUri, filePath);

        return video;
    }
	private async ValueTask AuthorizeAsync()
	{
        if (!_api.IsAuthorized)
        {
            await _api.AuthorizeAsync(new ApiAuthParams
            {
                ApplicationId = _options.Value.ApplicationId,
                Login = _options.Value.Login,
                Password = _options.Value.Password,
                Settings = Settings.Groups | Settings.Wall | Settings.Video,
            });
        }
    }

    public void Dispose()
    {
        _semaphoreSlim.Dispose();
        _api.Dispose();
    }
}
