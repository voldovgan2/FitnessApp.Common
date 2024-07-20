using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;

public class DbContextFixtureBase<TEntity> : TestBase, IDisposable
    where TEntity : IGenericEntity
{
    public DbContext<TEntity> DbContext { get; }
    private readonly MongoClient _mongoClient;
    private readonly MongoDbSettings _mongoDbSettings;

    public DbContextFixtureBase(string databaseName, Func<string, TEntity> createEntityFacotryMethod)
    {
        var objectSerializer = new ObjectSerializer(type => true);
        BsonSerializer.RegisterSerializer(objectSerializer);
        _mongoDbSettings = new MongoDbSettings
        {
            ConnectionString = Configuration.GetValue<string>("MongoConnection:ConnectionString"),
            DatabaseName = databaseName,
            CollecttionName = "Items",
        };
        _mongoClient = new MongoClient(_mongoDbSettings.ConnectionString);
        DbContext = new DbContext<TEntity>(_mongoClient, Options.Create(_mongoDbSettings));
        IEnumerable<string> itemIds =
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
