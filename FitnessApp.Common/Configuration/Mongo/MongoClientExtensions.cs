using System;
using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FitnessApp.Common.Configuration.Mongo
{
    public static class MongoClientExtensions
    {
        public static IServiceCollection ConfigureMongo(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.Configure<MongoDbSettings>(configuration.GetSection("MongoConnection"));
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
