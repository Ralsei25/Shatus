using Microsoft.Extensions.DependencyInjection;
using VkNet.Utils.AntiCaptcha;

namespace Shatus.Vk.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TrimSeconds(this DateTime date, long roundTicks)
    {
        return new DateTime(date.Ticks - date.Ticks % roundTicks, date.Kind);
    }
}
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShatusVk(this IServiceCollection services)
    {
        services.AddSingleton<VideoUploader>();
        services.AddSingleton<ShatusVkPublisher>();

        return services;
    }
    public static IServiceCollection AddConsoleCaptchaSolver(this IServiceCollection services)
    {
        services.AddSingleton<ICaptchaSolver, ConsoleCaptchaSolver>();

        return services;
    }
}
