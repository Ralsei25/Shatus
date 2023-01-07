using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shatus.WinForms.Configs;

namespace Shatus.WinForms.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureWritable<T>(this IServiceCollection services, IConfigurationSection section, string file = "appsettings.json", string? configsDirectoryPath = null) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(configsDirectoryPath))
            configsDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;

        services.Configure<T>(section);
        services.AddTransient<IWritableOptions<T>>(provider =>
        {
            var options = provider.GetRequiredService<IOptionsMonitor<T>>();
            return new WritableOptions<T>(configsDirectoryPath, options, section.Key, file);
        });
    }
}