namespace Shatus.YouTube;

public interface IVideoDownloader
{
    Task DownloadVideoAsync(string link, string outputFolder, string fileName);
}