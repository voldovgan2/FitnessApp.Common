using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;

public abstract class DbContextFixtureBase<TEntity> : TestBase, IDisposable
    where TEntity : IWithUserIdEntity
{
    public DbContext<TEntity> DbContext { get; }
    private readonly MongoClient _mongoClient;
    private readonly MongoDbSettings _mongoDbSettings;

    protected DbContextFixtureBase(Func<string, TEntity> createEntityFacotryMethod)
    {
        var objectSerializer = new ObjectSerializer(type => true);
        BsonSerializer.RegisterSerializer(objectSerializer);
        _mongoDbSettings = new MongoDbSettings
        {
            ConnectionString = Configuration.GetValue<string>("MongoConnection:ConnectionString"),
            DatabaseName = GetType().Name,
            CollecttionName = "Items",
        };
        _mongoClient = new MongoClient(_mongoDbSettings.ConnectionString);
        DbContext = new DbContext<TEntity>(_mongoClient, _mongoDbSettings);
        string[] itemIds =
        [
            TestData.EntityIdToGet,
            TestData.EntityIdToDelete,
            TestData.EntityIdToUpdate
        ];
        var createdItemsTasks = itemIds.Select(itemId => DbContext.CreateItem(createEntityFacotryMethod(itemId)));
        Task.WhenAll(createdItemsTasks).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _mongoClient?.DropDatabase(_mongoDbSettings.DatabaseName);
    }
}
