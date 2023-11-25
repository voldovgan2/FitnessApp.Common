using System;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FitnessApp.Common.Configuration.Mongo
{
    public static class MongoClientExtensions
    {
        public static IServiceCollection ConfigureMongoClient(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<IMongoClient, MongoClient>(
                sp =>
                {
                    var vaultService = sp.GetRequiredService<IVaultService>();
                    var connectionString = vaultService.GetSecret("MongoConnection:ConnectionString").GetAwaiter().GetResult();
                    return new MongoClient(connectionString);
                }
            );

            return services;
        }
    }
}
