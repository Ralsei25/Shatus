using YoutubeExplode;
using YoutubeExplode.Converter;

namespace Shatus.YouTube;

public class YoutubeDownloader : IVideoDownloader
{
    private readonly IHttpClientFactory _httpClientFactory;

    public YoutubeDownloader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task DownloadVideoAsync(string link, string outputFolder, string fileName)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var youtubeClient = new YoutubeClient(httpClient);
        await youtubeClient.Videos.DownloadAsync(link, $"{outputFolder}\\{fileName}");
    }
}
