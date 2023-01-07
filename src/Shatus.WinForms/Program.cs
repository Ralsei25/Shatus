using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shatus.Vk;
using Shatus.Vk.Extensions;
using Shatus.Youtube.Extensions;
using VkNet.Utils.AntiCaptcha;

namespace ShatusBot.WinForms;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();

        ConfigureServices(services);

        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        var shatusControl = serviceProvider.GetRequiredService<ShatusControlForm>();
        shatusControl.TopMost = true;
        shatusControl.TopLevel = true;

        WinFormCaptchaSolver.SynchronizationContext = SynchronizationContext.Current;

        Application.Run(shatusControl);
    }
    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<ShatusControlForm>();
        services.AddHttpClient();
        services.AddShatusVk();
        services.AddShatusYoutubeDownloader();
        services.AddSingleton<ICaptchaSolver, WinFormCaptchaSolver>();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets(typeof(Program).Assembly);

        services.Configure<VkConfigs>(builder.Build().GetSection(nameof(VkConfigs)));
    }
}