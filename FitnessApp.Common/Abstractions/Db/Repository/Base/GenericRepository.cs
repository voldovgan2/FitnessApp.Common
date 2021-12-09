using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Base;
using FitnessApp.Common.Abstractions.Models.Base;
using FitnessApp.Common.Logger;
using FitnessApp.Common.Serializer.JsonMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Db.Repository.Base
{
    public class GenericRepository<Entity, Model, CreateModel, UpdateModel> 
        : IGenericRepository<Entity, Model, CreateModel, UpdateModel>
        where Entity : IEntity
        where Model : ISearchableModel
        where CreateModel : ICreateModel
        where UpdateModel : IUpdateModel
    {
        private readonly DbContext<Entity> _dbContext;
        private readonly IJsonMapper _mapper;
        private readonly ILogger<GenericRepository<Entity, Model, CreateModel, UpdateModel>> _log;

        public GenericRepository
        (
            IOptions<CosmosDbSettings> settings, 
            IJsonMapper mapper, 
            ILogger<GenericRepository<Entity, Model, CreateModel, UpdateModel>> log
        )
        {
            _dbContext = new DbContext<Entity>(settings);
            _mapper = mapper;
            _log = log;
        }

        public virtual Task<IEnumerable<Model>> GetAllItemsAsync()
        {
            IEnumerable<Entity> items = null;
            try
            {
                items = _dbContext.Container.GetItemLinqQueryable<Entity>().ToList();
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            var result = items?.Select(entity => _mapper.Convert<Model>(entity));
            return Task.FromResult(result);
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
                result = _mapper.Convert<Model>(entity);
            }
            return Task.FromResult(result);
        }

        public virtual async Task<Model> CreateItemAsync(CreateModel model)
        {
            var itemExists = _dbContext.Container.GetItemLinqQueryable<Entity>().Where(i => i.UserId == model.UserId).Any();
            Model result = default;
            if (!itemExists)
            {
                var entity = _mapper.Convert<Entity>(model);
                Entity created = default;
                try
                {
                    created = (await _dbContext.Container.CreateItemAsync(entity, new PartitionKey(entity.UserId))).Resource;
                }
                catch (Exception ex)
                {
                    _log.WriteException(ex);
                }
                if (created != null)
                {
                    result = _mapper.Convert<Model>(created);
                }
            }
            else
            {
                _log.WriteWarning($"Item already exists. UserId: {model.UserId}");
            }
            return result;
        }

        public virtual async Task<Model> UpdateItemAsync(UpdateModel model)
        {
            Entity replaced = default;
            try
            {
                var entity = _dbContext.Container.GetItemLinqQueryable<Entity>().Where(i => i.UserId == model.UserId).Single();
                var propertiesToUpdate = model.GetType().GetProperties()
                    .Where(p => p.GetValue(model, null) != null);
                var entityProperties = entity.GetType().GetProperties();
                foreach (var propertyToUpdate in propertiesToUpdate)
                {
                    var entityProperty = entityProperties.FirstOrDefault(p => p.Name == propertyToUpdate.Name);
                    try 
                    {
                        entityProperty.SetValue(entity, propertyToUpdate.GetValue(model, null));
                    }
                    catch(Exception ex)
                    {
                        _log.WriteWarning($"Failed to update property: {entityProperty.Name}");
                        throw ex;
                    }
                }
                replaced = (await _dbContext.Container.UpsertItemAsync(entity, new PartitionKey(model.UserId))).Resource;                
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            Model result = default;
            if (replaced != null)
            {
                result = _mapper.Convert<Model>(replaced);
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