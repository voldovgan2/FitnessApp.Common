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
            ArgumentNullException.ThrowIfNull(services);

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
