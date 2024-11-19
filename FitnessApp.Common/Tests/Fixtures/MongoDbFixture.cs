using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace FitnessApp.Common.Tests.Fixtures;
public abstract class MongoDbFixture<TEntity> : IDisposable
    where TEntity : class
{
    protected IMongoCollection<TEntity> Collection { get; }

    protected MongoDbFixture(
        string connectionString,
        string databaseName,
        string collecttionName)
    {
        var database = new MongoClient(connectionString).GetDatabase(databaseName);
        if (!database.ListCollectionNames().ToList().Exists(c => c == collecttionName))
            database.CreateCollection(collecttionName);
        Collection = database.GetCollection<TEntity>(collecttionName);
    }

    public async Task SeedData(TEntity[] items)
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