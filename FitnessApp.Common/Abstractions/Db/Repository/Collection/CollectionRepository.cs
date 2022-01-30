using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Logger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
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
        private readonly IMapper _mapper;
        private readonly ILogger<CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> _log;

        public CollectionRepository
        (
            IOptions<CosmosDbSettings> settings,
            IMapper mapper,
            ILogger<CollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> log,
            JsonConverter[] converters
        )
        {
            _dbContext = new DbContext<Entity>(settings, converters);
            _mapper = mapper;
            _log = log;
        }

        public virtual Task<Model> GetItemByUserIdAsync(string userId)
        {
            Entity entity = default;
            try
            {
                entity = _dbContext.GetItemById(userId);
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            Model result = default;
            if (entity != null)
            {  
                result = _mapper.Map<Model>(entity);
            }
            return Task.FromResult(result);
        }

        public virtual async Task<string> CreateItemAsync(CreateModel model)
        {
            var itemExists = _dbContext.TryGetItemById(model.UserId) != null;
            Entity entity = default;
            if (!itemExists)
            {
                entity = _mapper.Map<Entity>(model);
                try
                {
                    entity = await _dbContext.CreateItem(entity);
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
                var entity = _dbContext.GetItemById(model.UserId);
                var collection = entity.Collection[model.CollectionName];
                CollectionItemEntity itemEntity = default;
                switch (model.Action)
                {
                    case UpdateCollectionAction.Add:
                        {
                            var added = _mapper.Map<CollectionItemEntity>(model.Model);
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
                                .Where(p => p.Name != nameof(model.Model.Id));
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
                            itemEntity = _mapper.Map<CollectionItemEntity>(updated);
                        }
                        break;
                    case UpdateCollectionAction.Remove:
                        {
                            var removed = collection.Single(i => i.Id == model.Model.Id);
                            collection.Remove(removed);
                            itemEntity = _mapper.Map<CollectionItemEntity>(removed);
                        }
                        break;
                }

                Entity replaced = await _dbContext.UpdateItem(entity);
                if (replaced != null)
                {
                    result = _mapper.Map<CollectionItemModel>(itemEntity);
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
                var deleted = await _dbContext.DeleteItem(userId);
                if (deleted)
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