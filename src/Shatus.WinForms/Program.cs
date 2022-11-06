using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShatusBot.Vk;
using ShatusBot.Vk.Extensions;
using ShatusBot.Youtube.Extensions;
using VkNet.Utils.AntiCaptcha;

namespace ShatusBot.WinForms
{
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

            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var form1 = serviceProvider.GetRequiredService<Form1>();
                Application.Run(form1);
            }
        }
        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddScoped<Form1>();
            services.AddHttpClient();
            services.AddShatusVk();
            services.AddShatusYoutube();
            services.AddSingleton<ICaptchaSolver, WinCaptchaSolver>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(typeof(Program).Assembly)
                .AddEnvironmentVariables();

            services.Configure<VkBotConfigs>(builder.Build().GetSection(nameof(VkBotConfigs)));
        }
    }
}