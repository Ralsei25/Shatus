using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shatus.Vk.Extensions;
using Shatus.Vk.Helpers;
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
    private readonly IVkApi _api;
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
            var message = $"#{videoName.Trim().Replace(" ", "")}";
            await SchedulePostAsync(publishDate, video, message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex.Message, ex);
            throw;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task SchedulePostAsync(DateTime publishDate, Video video, string message)
    {
        await _api.Wall.PostAsync(new()
        {
            OwnerId = -_options.Value.GroupId,
            FromGroup = true,
            Signed = false,
            PublishDate = publishDate,
            Attachments = new MediaAttachment[] { video },
            Message = message,
        });
    }

    // TODO: Refactoring requared
    private async Task<DateTime> GetPublishDateAsync()
    {
        var publishDate = TimeHelper.MoscowNow.AddHours(1).TrimSeconds();

        var wall = await _api.Wall.GetAsync(new()
        {
            OwnerId = -_options.Value.GroupId,
            Filter = WallFilter.Postponed,
            Count = 100,
        });

        bool postTomorow = publishDate.IsNightTime();
        
        var lastPostToday = wall.WallPosts
            .Where(p => p.Date.Value.Date == (postTomorow ? DateTime.Today.AddDays(1) : DateTime.Today))
            .OrderByDescending(p => p.Date)
            .FirstOrDefault();


        if (lastPostToday is not null)
        {
            publishDate = lastPostToday.Date.Value.UtcToMoscow().AddHours(4).TrimSeconds();
            while (publishDate.IsNightTime())
                publishDate = publishDate.AddHours(1);
            while (wall.WallPosts.Any(p => p.Date.Value.UtcToMoscow().TrimSeconds() == publishDate) ||
                    publishDate.IsNightTime()) 
                publishDate = publishDate.AddHours(4);
        }
        else
        {
            while (publishDate.IsNightTime())
                publishDate = publishDate.AddHours(1);
        }
        return publishDate.MoscowToUtc();
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
