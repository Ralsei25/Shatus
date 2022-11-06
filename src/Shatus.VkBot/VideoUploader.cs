using RestSharp;
using System.Text.Json.Serialization;

namespace Shatus.Vk;

public struct VideoInfo
{
    [JsonPropertyName("video_id")] public long VideoId { get; set; }
}
public class VideoUploader
{
    private readonly IHttpClientFactory _httpClientFactory;

    public VideoUploader(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task<VideoInfo> UploadVideoAsync(string uploadLink, string filePath)
    {
        using var client = new RestClient(_httpClientFactory.CreateClient());
        var request = new RestRequest(uploadLink, Method.Post);
        request.AddHeader("Content-Type", "multipart/form-data");
        request.AddFile("video_file", filePath, "video/mp4");
        var response = await client.ExecuteAsync<VideoInfo>(request);
        return response.Data;
    }
}