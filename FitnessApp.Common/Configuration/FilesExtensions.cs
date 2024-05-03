using System;
using FitnessApp.Common.Files;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MongoDB.Driver;

namespace FitnessApp.Common.Configuration
{
    public static class FilesExtensions
    {
        public static IServiceCollection ConfigureFilesService(this IServiceCollection services, IConfiguration configuration)
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

            services.AddTransient<IFilesService, FilesService>();

            return services;
        }
    }
}
