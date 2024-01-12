using System;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace FitnessApp.Common.Configuration.Files
{
    public static class FilesExtensions
    {
        public static IServiceCollection AddFilesService(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            var vaultService = services.BuildServiceProvider().GetRequiredService<IVaultService>();
            var endpoint = configuration.GetValue<string>("Minio:Endpoint");
            var accessKey = configuration.GetValue<string>("Minio:AccessKey");
            var secretKey = vaultService.GetSecret("Minio:SecretKey").GetAwaiter().GetResult();
            var secure = configuration.GetValue<bool>("Minio:Secure");
            services.AddMinio(options =>
            {
                options.WithEndpoint(endpoint);
                options.WithCredentials(accessKey, secretKey);
                options.WithSSL(secure);
                options.Build();
            });

            return services;
        }
    }
}
