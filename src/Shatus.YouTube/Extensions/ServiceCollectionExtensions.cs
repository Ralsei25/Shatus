using Microsoft.Extensions.DependencyInjection;
using Shatus.YouTube;

namespace Shatus.Youtube.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShatusYoutubeDownloader(this IServiceCollection services)
    {
        services.AddSingleton<IVideoDownloader, YoutubeDownloader>();

        return services;
    }
}
