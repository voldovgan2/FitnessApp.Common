using System;
using System.Reflection;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;
using MongoDB.Driver;

namespace FitnessApp.Common.Helpers;
public static class DbContextHelper
{
    public static async Task<TEntity[]> FilterCollection<TEntity>(IMongoCollection<TEntity> collection, FilterDefinition<TEntity> filter)
        where TEntity : IGenericEntity
    {
        return [.. await (await collection.FindAsync(filter)).ToListAsync()];
    }

    public static async Task<PagedDataModel<TEntity>> GetPagedCollection<TEntity>(
        IMongoCollection<TEntity> collection,
        FilterDefinition<TEntity> filter,
        GetPagedDataModel model)
        where TEntity : IGenericEntity
    {
        var data = collection.Find(filter);
        if (!string.IsNullOrWhiteSpace(model.SortBy))
        {
            PropertyInfo propertyInfo = typeof(TEntity).GetProperty(model.SortBy);
            if (propertyInfo != null)
            {
                data = model.Asc ? data.SortBy((TEntity x) => propertyInfo.GetValue(x)) : data.SortByDescending((TEntity x) => propertyInfo.GetValue(x));
            }
        }

        var totalCount = await collection.CountDocumentsAsync(filter);
        return new PagedDataModel<TEntity>
        {
            Items = [..
                await data
                    .Skip(model.Page * model.PageSize)
                    .Limit(model.PageSize)
                    .ToListAsync()
            ],
            Page = model.Page,
            TotalCount = (int)totalCount,
        };
    }

    public static bool IsConfirmed(bool isAcknowledged, long confirmedCount)
    {
        return isAcknowledged && confirmedCount == 1;
    }

    public static FilterDefinition<TEntity> CreateGetByUserIdFiter<TEntity>(string userId)
        where TEntity : IWithUserIdEntity
    {
        return Builders<TEntity>.Filter.Eq(s => s.UserId, userId);
    }

    public static FilterDefinition<TEntity> CreateGetByUserIdsFiter<TEntity>(string[] userIds)
        where TEntity : IWithUserIdEntity
    {
        return Builders<TEntity>.Filter.In(i => i.UserId, userIds);
    }

    public static FilterDefinition<TPartitionKey> CreateGetByPartitionKeyFiter<TPartitionKey>(string partitionKey)
        where TPartitionKey : IPartitionKey
    {
        return Builders<TPartitionKey>.Filter.Eq(s => s.PartitionKey, partitionKey);
    }

    public static FilterDefinition<TEntity> CreateGetByArrayParamsFiter<TParams, TEntity>(
        TParams[] @params,
        Func<TParams, FilterDefinition<TEntity>> createGetByArrayParamFiter)
        where TParams : IMultipleParamFilter
        where TEntity : IGenericEntity
    {
        if (@params.Length == 0)
            return Builders<TEntity>.Filter.Where(o => false);
        var filter = createGetByArrayParamFiter(@params[0]);
        for (int k = 1; k < @params.Length; k++)
        {
            filter = Builders<TEntity>.Filter.Or(filter, createGetByArrayParamFiter(@params[k]));
        }

        return filter;
    }
}
