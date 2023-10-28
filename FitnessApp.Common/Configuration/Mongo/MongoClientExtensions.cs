using System;
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
                    var connectionString = configuration.GetValue<string>("MongoConnection:ConnectionString");
                    return new MongoClient(connectionString);
                }
            );

            return services;
        }
    }
}
