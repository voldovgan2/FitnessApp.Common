using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace FitnessApp.Common.Tests.Fixtures;
public abstract class MongoDbFixture<TEntity> : IDisposable
    where TEntity : class
{
    protected IMongoCollection<TEntity> Collection { get; }

    protected MongoDbFixture(TEntity[] items)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        var database = new MongoClient(configuration["ConnectionString"]).GetDatabase(configuration["DatabaseName"]);
        if (!database.ListCollectionNames().ToList().Exists(c => c == configuration["CollecttionName"]))
            database.CreateCollection(configuration["CollecttionName"]);
        Collection = database.GetCollection<TEntity>(configuration["CollecttionName"]);
        SeedData(items).GetAwaiter().GetResult();
    }

    private async Task SeedData(TEntity[] items)
    {
        var createdItemsTasks = items.Select(item => Collection.InsertOneAsync(item));
        await Task.WhenAll(createdItemsTasks);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            Collection.DeleteMany(e => true);
    }
}