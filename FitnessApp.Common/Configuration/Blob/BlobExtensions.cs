using System;
using FitnessApp.Common.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Configuration.Blob
{
    public static class BlobExtensions
    {
        public static IServiceCollection AddBlobService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<IBlobService, BlobService>(
                sp =>
                {
                    var connectionString = configuration.GetValue<string>("BlobSettings:ConnectionString");
                    return new BlobService(connectionString);
                }
            );

            return services;
        }
    }
}
