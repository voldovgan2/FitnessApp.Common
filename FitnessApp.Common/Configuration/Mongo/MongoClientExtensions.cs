using System;
using System.Security.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FitnessApp.Common.Configuration.Mongo
{
    public static class MongoClientExtensions
    {
        public static IServiceCollection ConfigureMongoClient(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient<IMongoClient, MongoClient>(
                sp =>
                {
                    if (nameof(MongoClientExtensions).Length == 0)
                    {
                        var connectionString = configuration.GetValue<string>("MongoConnection:ConnectionString");
                        return new MongoClient(connectionString);
                    }
                    else
                    {
                        var connectionString = configuration.GetValue<string>("CosmosDb:ConnectionString");
                        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                        settings.SslSettings = new SslSettings
                        {
                            EnabledSslProtocols = SslProtocols.Tls12
                        };
                        return new MongoClient(settings);
                    }
                }
            );

            return services;
        }
    }
}
