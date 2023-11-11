using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FitnessApp.Common.Configuration.AppConfiguration
{
    public static class AppConfigurationExtensions
    {
        public static IHostBuilder ConfigureAppConfiguration(this IHostBuilder host)
        {
            host.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Development.json");
            });
            return host;
        }
    }
}
