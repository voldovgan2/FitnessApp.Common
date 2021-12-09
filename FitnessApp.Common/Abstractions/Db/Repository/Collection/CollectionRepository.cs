using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Logger;
using FitnessApp.Common.Serializer.JsonMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Db.Repository.Collection
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
            IOptions<CosmosDbSettings> settings,
            IJsonMapper mapper,
            ILogger<CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> log
        )
        {
            _dbContext = new DbContext<Entity>(settings);
            _mapper = mapper;
            _log = log;
        }

        public virtual Task<Model> GetItemByUserIdAsync(string userId)
        {
            Entity entity = default;
            try
            {
                entity = _dbContext.Container.GetItemLinqQueryable<Entity>().Where(i => i.UserId == userId).Single();
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
            return Task.FromResult(result);
        }

        public virtual async Task<string> CreateItemAsync(CreateModel model)
        {
            var itemExists = _dbContext.Container.GetItemLinqQueryable<Entity>().Where(i => i.UserId == model.UserId).Any();
            Entity entity = default;
            if (!itemExists)
            {
                entity = _mapper.Convert<Entity>(model);
                try
                {
                    entity = (await _dbContext.Container.CreateItemAsync(entity, new PartitionKey(entity.UserId))).Resource;
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
                var entity = _dbContext.Container.GetItemLinqQueryable<Entity>().Where(i => i.UserId == model.UserId).Single();
                var collection = entity.Collection[model.CollectionName];
                CollectionItemEntity itemEntity = default;
                switch (model.Action)
                {
                    case UpdateCollectionAction.Add:
                        {
                            var added = _mapper.Convert<CollectionItemEntity>(model.Model);
                            if (added.Id == null)
                            {
                                added.Id = Guid.NewGuid().ToString();
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

                Entity replaced = (await _dbContext.Container.UpsertItemAsync(entity, new PartitionKey(model.UserId))).Resource;
                if (replaced != null)
                {
                    result = _mapper.Convert<CollectionItemModel>(replaced);
                }
                else
                {
                    var internalError = $"Failed to update item. UserId: {model.UserId}";
                    _log.WriteWarning(internalError);
                    throw new Exception(internalError);
                }
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            return result;
        }

        public virtual async Task<string> DeleteItemAsync(string userId)
        {
            string result = null;
            try
            {
                var deleted = (await _dbContext.Container.DeleteItemAsync<Entity>(userId, new PartitionKey(userId))).Resource;
                if (deleted != null)
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
    }
}