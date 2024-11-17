using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Exceptions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FitnessApp.Common.Abstractions.Db.DbContext;

public class DbContext<TGenericEntity> : IDbContext<TGenericEntity>
    where TGenericEntity : IGenericEntity
{
    private readonly IMongoCollection<TGenericEntity> _collection;
    private readonly MongoDbSettings _settings;

    public DbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        var database = mongoClient.GetDatabase(_settings.DatabaseName);
        if (!database.ListCollectionNames().ToList().Exists(c => c == _settings.CollecttionName))
            database.CreateCollection(_settings.CollecttionName);
        _collection = database.GetCollection<TGenericEntity>(_settings.CollecttionName);
    }

    public async Task<TGenericEntity> GetItemById(string id)
    {
        var result = await FilterCollection(CreateGetByIdFiter(id));
        return result.FirstOrDefault() ?? throw new EntityNotFoundException(_settings.CollecttionName, id);
    }

    public Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids)
    {
        return FilterCollection(Builders<TGenericEntity>.Filter.In(i => i.UserId, ids));
    }

    public async Task<TGenericEntity> CreateItem(TGenericEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        return await GetItemById(entity.UserId);
    }

    public async Task<TGenericEntity> UpdateItem(TGenericEntity entity)
    {
        var replaceResult = await _collection.ReplaceOneAsync(CreateGetByIdFiter(entity.UserId), entity);
        return IsConfirmed(replaceResult.IsAcknowledged, replaceResult.MatchedCount)
            ? await GetItemById(entity.UserId)
            : throw new EntityNotFoundException(_settings.CollecttionName, entity.UserId);
    }

    public async Task<TGenericEntity> DeleteItem(string id)
    {
        var deleted = await GetItemById(id);
        var deleteResult = await _collection.DeleteOneAsync(CreateGetByIdFiter(id));
        return IsConfirmed(deleteResult.IsAcknowledged, deleteResult.DeletedCount)
            ? deleted
            : throw new EntityNotFoundException(_settings.CollecttionName, id);
    }

    private FilterDefinition<TGenericEntity> CreateGetByIdFiter(string id)
    {
        return Builders<TGenericEntity>.Filter.Eq(s => s.UserId, id);
    }

    private async Task<IEnumerable<TGenericEntity>> FilterCollection(FilterDefinition<TGenericEntity> filter)
    {
        return await (await _collection.FindAsync(filter)).ToListAsync();
    }

    private bool IsConfirmed(bool isAcknowledged, long confirmedCount)
    {
        return isAcknowledged && confirmedCount == 1;
    }
}