using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace FitnessApp.Common.Configuration.Files
{
    public static class FilesExtensions
    {
        public static IServiceCollection AddFilesService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<IMinioClient, MinioClient>(
                sp =>
                {
                    var endpoint = configuration.GetValue<string>("Minio:Endpoint");
                    var accessKey = configuration.GetValue<string>("Minio:AccessKey");
                    var secretKey = configuration.GetValue<string>("Minio:SecretKey");
                    var secure = configuration.GetValue<bool>("Minio:Secure");
                    return new MinioClient()
                        .WithEndpoint(endpoint)
                        .WithCredentials(accessKey, secretKey)
                        .WithSSL(secure)
                        .Build();
                }
            );

            return services;
        }
    }
}
