using FitnessApp.Abstractions.Db.Configuration;
using FitnessApp.Abstractions.Db.DbContext;
using FitnessApp.Abstractions.Db.Entities.Collection;
using FitnessApp.Abstractions.Db.Enums.Collection;
using FitnessApp.Abstractions.Models.Collection;
using FitnessApp.Logger;
using FitnessApp.Serializer.JsonMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Db.Repository.Collection
{
    public class CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        : ICollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        where Entity : ICollectionEntity
        where CollectionItemEntity : ICollectionItemEntity
        where Model : ICollectionModel
        where CollectionItemModel : ISearchableCollectionItemModel
        where CreateModel : ICreateCollectionModel
        where UpdateModel : IUpdateCollectionModel
    {
        private readonly DbContext<Entity> _dbContext;
        private readonly IJsonMapper _mapper;
        private const string _idPropertyName = "Id";
        private readonly ILogger<CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> _log;

        public CollectionRepository
        (
            IOptions<MongoDbSettings> settings,
            IJsonMapper mapper,
            ILogger<CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> log
        )
        {
            _dbContext = new DbContext<Entity>(settings);
            _mapper = mapper;
            _log = log;
        }

        public virtual async Task<Model> GetItemByUserIdAsync(string userId)
        {
            Entity entity = default;
            try
            {
                entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == userId))).Single();
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            Model result = default;
            if (entity != null)
            {  
                result = _mapper.Convert<Model>(entity, new string[] { nameof(entity.Collection) });
                result.Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>();
                foreach(var kvp in entity.Collection)
                {
                    var items = _mapper.Convert<IEnumerable<CollectionItemModel>>(kvp.Value);
                    var collectionItems = new List<ICollectionItemModel>();
                    foreach (var item in items)
                    {
                        collectionItems.Add(item);
                    }
                    result.Collection.Add(kvp.Key, collectionItems);
                }
            }
            return result;
        }

        public virtual async Task<string> CreateItemAsync(CreateModel model)
        {
            var itemExists = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Any();
            Entity entity = default;
            if (!itemExists)
            {
                entity = _mapper.Convert<Entity>(model);
                try
                {
                    await _dbContext.Collection.InsertOneAsync(entity);
                    entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Single();
                }
                catch (Exception ex)
                {
                    _log.WriteException(ex);
                }
            }
            else
            {
                _log.WriteWarning($"Item already exists. UserId: {model.UserId}");
            }
            string result = entity?.UserId;
            return result;
        }

        public virtual async Task<CollectionItemModel> UpdateItemAsync(UpdateModel model)
        {
            CollectionItemModel result = default;
            try
            {
                result = await UnsafeUpdateItemAsync(model);
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            return result;
        }

        public virtual async Task<string> UpdateItemsAsync(IEnumerable<UpdateModel> models)
        {
            var updateResult = await _dbContext.ExecuteTransacion(async () =>
            {
                foreach (var model in models)
                {
                    await UnsafeUpdateItemAsync(model);
                }
            });
            if (updateResult != null)
            {
                _log.WriteWarning($"Error updating items: {updateResult}");
            }
            return updateResult;
        }

        public virtual async Task<string> DeleteItemAsync(string userId)
        {
            string result = null;
            try
            {
                var entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == userId))).Single();
                var deleteResult = await _dbContext.Collection.DeleteOneAsync(Builders<Entity>.Filter.Eq(s => s.UserId, userId));
                if (deleteResult.IsAcknowledged && deleteResult.DeletedCount == 1)
                {
                    result = userId;
                }
                else
                {
                    _log.WriteWarning($"Failed to delete item. UserId: {userId}");
                }
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            return result;
        }

        private async Task<CollectionItemModel> UnsafeUpdateItemAsync(UpdateModel model)
        {
            CollectionItemModel result = default;
            var entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Single();
            var collection = entity.Collection[model.CollectionName];
            CollectionItemEntity itemEntity = default;
            switch (model.Action)
            {
                case UpdateCollectionAction.Add:
                    {
                        var added = _mapper.Convert<CollectionItemEntity>(model.Model);
                        if (added.Id == null)
                        {
                            added.Id = ObjectId.GenerateNewId().ToString();
                        }
                        collection.Add(added);
                        itemEntity = added;
                    }
                    break;
                case UpdateCollectionAction.Update:
                    {
                        var updated = collection.Single(i => i.Id == model.Model.Id);
                        var propertiesToUpdate = model.Model.GetType().GetProperties()
                            .Where(p => p.GetValue(model.Model, null) != null)
                            .Where(p => p.Name.ToUpper() != _idPropertyName.ToUpper());
                        var entityProperties = typeof(CollectionItemEntity).GetProperties();
                        foreach (var propertyToUpdate in propertiesToUpdate)
                        {
                            var entityProperty = entityProperties.FirstOrDefault(p => p.Name == propertyToUpdate.Name);
                            try
                            {
                                entityProperty.SetValue(updated, propertyToUpdate.GetValue(model.Model, null));
                            }
                            catch (Exception ex)
                            {
                                _log.WriteWarning($"Failed to update property: {entityProperty.Name}");
                                throw ex;
                            }
                        }
                        itemEntity = _mapper.Convert<CollectionItemEntity>(updated);
                    }
                    break;
                case UpdateCollectionAction.Remove:
                    {
                        var removed = collection.Single(i => i.Id == model.Model.Id);
                        collection.Remove(removed);
                        itemEntity = _mapper.Convert<CollectionItemEntity>(removed);
                    }
                    break;
            }

            var replaceResult = await _dbContext.Collection.ReplaceOneAsync(s => s.UserId == model.UserId, entity);
            if (replaceResult.IsAcknowledged && replaceResult.ModifiedCount == 1)
            {
                entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Single();
                result = _mapper.Convert<CollectionItemModel>(itemEntity);
            }
            else
            {
                var internalError = $"Failed to update item. UserId: {model.UserId}";
                _log.WriteWarning(internalError);
                throw new Exception(internalError);
            }
            return result;
        }
    }
}