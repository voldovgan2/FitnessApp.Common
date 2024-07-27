using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Exceptions;

namespace FitnessApp.Common.Abstractions.Db.Repository.Collection;

public abstract class CollectionRepository<
    TCollectionEntity,
    TCollectionItemEntity,
    TCollectionModel,
    TCollectionItemModel,
    TCreateCollectionModel,
    TUpdateCollectionModel>(IDbContext<TCollectionEntity> dbContext, IMapper mapper)
    : ICollectionRepository<
        TCollectionModel,
        TCollectionItemModel,
        TCreateCollectionModel,
        TUpdateCollectionModel>
    where TCollectionEntity : ICollectionEntity
    where TCollectionItemEntity : ICollectionItemEntity
    where TCollectionModel : ICollectionModel
    where TCollectionItemModel : ICollectionItemModel
    where TCreateCollectionModel : ICreateCollectionModel
    where TUpdateCollectionModel : IUpdateCollectionModel
{
    protected readonly IDbContext<TCollectionEntity> _dbContext = dbContext;
    protected readonly IMapper _mapper = mapper;

    public async Task<TCollectionModel> GetItemByUserId(string userId)
    {
        var entity = await _dbContext.GetItemById(userId);
        var result = _mapper.Map<TCollectionModel>(entity);
        return result;
    }

    public async Task<IEnumerable<TCollectionItemModel>> GetCollectionByUserId(string userId, string collectionName)
    {
        var entity = await _dbContext.GetItemById(userId);
        var result = _mapper.Map<IEnumerable<TCollectionItemModel>>(entity.Collection[collectionName]);
        return result;
    }

    public async Task<string> CreateItem(TCreateCollectionModel model)
    {
        var entity = _mapper.Map<TCollectionEntity>(model);
        entity = await _dbContext.CreateItem(entity);
        string result = entity.UserId;
        return result;
    }

    public async Task<TCollectionItemModel> UpdateItem(TUpdateCollectionModel model)
    {
        var entity = await _dbContext.GetItemById(model.UserId);
        var updateItemCollectionResult = UpdateItemCollection(entity, model);
        await _dbContext.UpdateItem(updateItemCollectionResult.Item1);
        var result = _mapper.Map<TCollectionItemModel>(updateItemCollectionResult.Item2);
        return result;
    }

    public async Task UpdateItems(IEnumerable<TUpdateCollectionModel> models)
    {
        var distinctEntities = await _dbContext.GetItemsByIds(models.Select(m => m.UserId).Distinct());
        var entitiesToUpdte = models.Select(model => UpdateItemCollection(distinctEntities.Single(e => e.UserId == model.UserId), model).Item1);
        await _dbContext.UpdateItems(entitiesToUpdte);
    }

    public async Task<TCollectionModel> DeleteItem(string userId)
    {
        var deleted = await _dbContext.DeleteItem(userId);
        var result = _mapper.Map<TCollectionModel>(deleted);
        return result;
    }

    private Tuple<TCollectionEntity, TCollectionItemEntity> UpdateItemCollection(TCollectionEntity entity, TUpdateCollectionModel model)
    {
        var collection = entity.Collection[model.CollectionName];
        TCollectionItemEntity itemEntity = default;
        switch (model.Action)
        {
            case UpdateCollectionAction.Add:
                {
                    if (collection.Exists(i => i.Id == model.Model.Id))
                        throw new DuplicateCollectionItemException(model.Model.Id);
                    var added = _mapper.Map<TCollectionItemEntity>(model.Model);
                    collection.Add(added);
                    itemEntity = added;
                }

                break;
            case UpdateCollectionAction.Update:
                {
                    var updated = collection.Single(i => i.Id == model.Model.Id);
                    var propertiesToUpdate = model.Model.GetType().GetProperties()
                        .Where(p => p.GetValue(model.Model, null) != null)
                        .Where(p => p.Name != nameof(model.Model.Id));
                    var entityProperties = typeof(TCollectionItemEntity).GetProperties();
                    foreach (var propertyToUpdate in propertiesToUpdate)
                    {
                        var entityProperty = Array.Find(entityProperties, p => p.Name == propertyToUpdate.Name);
                        entityProperty.SetValue(updated, propertyToUpdate.GetValue(model.Model, null));
                    }

                    itemEntity = _mapper.Map<TCollectionItemEntity>(updated);
                }

                break;
            case UpdateCollectionAction.Remove:
                {
                    var removed = collection.Single(i => i.Id == model.Model.Id);
                    collection.Remove(removed);
                    itemEntity = _mapper.Map<TCollectionItemEntity>(removed);
                }

                break;
        }

        return new Tuple<TCollectionEntity, TCollectionItemEntity>(entity, itemEntity);
    }
}