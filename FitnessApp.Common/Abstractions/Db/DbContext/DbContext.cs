using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FitnessApp.Common.Abstractions.Db.DbContext;

public class DbContext<TGenericEntity> : IDbContext<TGenericEntity>
    where TGenericEntity : IGenericEntity
{
    private readonly IMongoClient _mongoClient;
    private readonly IMongoCollection<TGenericEntity> _collection;

    public DbContext(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        _mongoClient = mongoClient;
        var database = _mongoClient.GetDatabase(settings.Value.DatabaseName);
        if (!database.ListCollectionNames().ToList().Exists(c => c == settings.Value.CollecttionName))
        {
            database.CreateCollection(settings.Value.CollecttionName);
        }

        _collection = database.GetCollection<TGenericEntity>(settings.Value.CollecttionName);
    }

    public async Task<TGenericEntity> GetItemById(string id)
    {
        return (await GetItemsByIds([id])).Single();
    }

    public async Task<TGenericEntity> TryGetItemById(string id)
    {
        return (await GetItemsByIds([id])).SingleOrDefault();
    }

    public async Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids)
    {
        var result = await (await _collection.FindAsync(Builders<TGenericEntity>.Filter.Where(i => ids.Contains(i.UserId)))).ToListAsync();
        return result;
    }

    public Task<IEnumerable<TGenericEntity>> FilterItems(Expression<Func<TGenericEntity, bool>> predicate)
    {
        var result = _collection
            .AsQueryable()
            .Where(predicate)
            .AsEnumerable();
        return Task.FromResult(result);
    }

    public async Task<TGenericEntity> CreateItem(TGenericEntity entity)
    {
        await _collection.InsertOneAsync(entity);
        var result = await GetItemById(entity.UserId);
        return result;
    }

    public async Task<TGenericEntity> UpdateItem(TGenericEntity entity)
    {
        var replaceResult = await _collection.ReplaceOneAsync(s => s.UserId == entity.UserId, entity);
        return replaceResult.IsAcknowledged && replaceResult.MatchedCount == 1
            ? await GetItemById(entity.UserId)
            : throw;
    }

    public async Task UpdateItems(IEnumerable<TGenericEntity> entities)
    {
        using (var session = await _mongoClient.StartSessionAsync())
        {
            session.StartTransaction();
            try
            {
                foreach (var entity in entities)
                {
                    await UpdateItem(entity);
                }

                await session.CommitTransactionAsync();
            }
            finally
            {
                await session.AbortTransactionAsync();
            }
        }
    }

    public async Task<TGenericEntity> DeleteItem(string userId)
    {
        var deleted = await GetItemById(userId);
        var deleteResult = await _collection.DeleteOneAsync(Builders<TGenericEntity>.Filter.Eq(s => s.UserId, userId));
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1
            ? deleted
            : default;
    }
}