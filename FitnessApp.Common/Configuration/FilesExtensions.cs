using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Files;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace FitnessApp.Common.Configuration;

[ExcludeFromCodeCoverage]
public static class FilesExtensions
{
    public static IServiceCollection ConfigureFilesService(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient(
            sp =>
            {
                var vaultService = services.BuildServiceProvider().GetRequiredService<IVaultService>();
                var endpoint = configuration.GetValue<string>("Minio:Endpoint");
                var accessKey = configuration.GetValue<string>("Minio:AccessKey");
                var secretKey = vaultService.GetSecret("Minio:SecretKey").GetAwaiter().GetResult();
                var secure = configuration.GetValue<bool>("Minio:Secure");
                return new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithSSL(secure)
                    .Build();
            }
        );

        services.AddTransient<IFilesService, FilesService>();

        return services;
    }
}
