using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace FitnessApp.Common.Configuration;

[ExcludeFromCodeCoverage]
public static class MongoClientExtensions
{
    public static IServiceCollection ConfigureMongo(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.Configure<MongoDbSettings>(configuration.GetSection("MongoConnection"));
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
