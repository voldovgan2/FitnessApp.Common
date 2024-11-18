using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using Mongo2Go;
using MongoDB.Driver;

namespace FitnessApp.Common.Tests.Fixtures;
public abstract class MongoDbFixtureBase<TEntity> : IDisposable
    where TEntity : IWithUserIdEntity
{
    private readonly MongoDbRunner _runner;
    public MongoClient Client { get; }

    protected MongoDbFixtureBase()
    {
        _runner = MongoDbRunner.Start();
        Client = new MongoClient(_runner.ConnectionString);
    }

    public async Task SeedData(
        string databaseName,
        string collecttionName,
        string[] ids)
    {
        var dbContext = new DbContext<TEntity>(Client, new MongoDbSettings
        {
            DatabaseName = databaseName,
            CollecttionName = collecttionName,
        });
        var createdItemsTasks = ids.Select(id => dbContext.CreateItem(CreateEntity(id)));
        await Task.WhenAll(createdItemsTasks);
    }

    protected virtual TEntity CreateEntity(string id)
    {
        var entity = Activator.CreateInstance<TEntity>();
        entity.UserId = id;
        return entity;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _runner.Dispose();
    }
}