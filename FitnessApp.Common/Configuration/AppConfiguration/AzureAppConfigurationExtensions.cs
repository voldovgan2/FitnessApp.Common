using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FitnessApp.Common.Configuration.AppConfiguration
{
    public static class AzureAppConfigurationExtensions
    {
        public static IHostBuilder ConfigureAzureAppConfiguration(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                var settings = config.Build();
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["ConnectionStrings:AppConfig"]);
                });
                config.AddJsonFile("appsettings.Development.json");
            });
            return host;
        }
    }
}
