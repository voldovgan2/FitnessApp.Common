using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        var mongoConnectionSection = configuration.GetSection("MongoConnection");
        var mongoDatabse = mongoConnectionSection.GetSection("DatabaseName").Value!;
        var connectionString = mongoConnectionSection.GetSection("ConnectionString").Value!;
        var contexts = configuration
            .GetSection("Contexts")
            .GetChildren()
            .Select(value => value.GetValue<string>("CollecttionName"));
        foreach (var context in contexts)
        {
            services.Configure<MongoDbSettings>(context, options =>
            {
                options.ConnectionString = connectionString;
                options.DatabaseName = mongoDatabse;
                options.CollecttionName = context;
            });
        }

        services.AddSingleton<IMongoClient, MongoClient>((IServiceProvider sp) => new MongoClient(connectionString));
        return services;
    }
}
