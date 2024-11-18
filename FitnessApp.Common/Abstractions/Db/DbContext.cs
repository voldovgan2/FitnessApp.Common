using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Exceptions;
using FitnessApp.Common.Helpers;
using MongoDB.Driver;

namespace FitnessApp.Common.Abstractions.Db;

public interface IDbContext<TEntity>
    where TEntity : IWithUserIdEntity
{
    Task<TEntity> GetByUserId(string userId);
    Task<TEntity[]> GetByUserIds(string[] userIds);
    Task<TEntity> CreateItem(TEntity entity);
    Task<TEntity> UpdateItem(TEntity entity);
    Task<TEntity> DeleteItem(string userId);
}

public abstract class DbContextBase<TEntity>
    where TEntity : IGenericEntity
{
    protected IMongoCollection<TEntity> Collection { get; }
    private protected MongoDbSettings Settings { get; }
    protected DbContextBase(IMongoClient mongoClient, MongoDbSettings settings)
    {
        Settings = settings;
        var database = mongoClient.GetDatabase(Settings.DatabaseName);
        if (!database.ListCollectionNames().ToList().Exists(c => c == Settings.CollecttionName))
            database.CreateCollection(Settings.CollecttionName);
        Collection = database.GetCollection<TEntity>(Settings.CollecttionName);
    }
}

public class DbContext<TEntity>(IMongoClient mongoClient, MongoDbSettings settings) :
    DbContextBase<TEntity>(mongoClient, settings),
    IDbContext<TEntity>
    where TEntity : IWithUserIdEntity
{
    public async Task<TEntity> GetByUserId(string userId)
    {
        var items = await DbContextHelper.FilterCollection(
            Collection,
            DbContextHelper.CreateGetByUserIdFiter<TEntity>(userId));
        return items.FirstOrDefault() ?? throw new EntityNotFoundException(Settings.CollecttionName, userId);
    }

    public async Task<TEntity[]> GetByUserIds(string[] userIds)
    {
        return await DbContextHelper.FilterCollection(
            Collection,
            DbContextHelper.CreateGetByUserIdsFiter<TEntity>(userIds));
    }

    public async Task<TEntity> CreateItem(TEntity entity)
    {
        await Collection.InsertOneAsync(entity);
        return await GetByUserId(entity.UserId);
    }

    public async Task<TEntity> UpdateItem(TEntity entity)
    {
        var replaceResult = await Collection.ReplaceOneAsync(DbContextHelper.CreateGetByUserIdFiter<TEntity>(entity.UserId), entity);
        return DbContextHelper.IsConfirmed(replaceResult.IsAcknowledged, replaceResult.MatchedCount)
            ? await GetByUserId(entity.UserId)
            : throw new EntityNotFoundException(Settings.CollecttionName, entity.UserId);
    }

    public async Task<TEntity> DeleteItem(string userId)
    {
        var deleted = await GetByUserId(userId);
        var deleteResult = await Collection.DeleteOneAsync(DbContextHelper.CreateGetByUserIdFiter<TEntity>(userId));
        return DbContextHelper.IsConfirmed(deleteResult.IsAcknowledged, deleteResult.DeletedCount)
            ? deleted
            : throw new EntityNotFoundException(Settings.CollecttionName, userId);
    }
}