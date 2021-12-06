using FitnessApp.Abstractions.Db.Configuration;
using FitnessApp.Abstractions.Db.DbContext;
using FitnessApp.Abstractions.Db.Entities.Base;
using FitnessApp.Abstractions.Models.Base;
using FitnessApp.Logger;
using FitnessApp.Serializer.JsonMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Db.Repository.Base
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
            IOptions<MongoDbSettings> settings, 
            IJsonMapper mapper, 
            ILogger<GenericRepository<Entity, Model, CreateModel, UpdateModel>> log
        )
        {
            _dbContext = new DbContext<Entity>(settings);
            _mapper = mapper;
            _log = log;
        }

        public virtual async Task<IEnumerable<Model>> GetAllItemsAsync()
        {
            IEnumerable<Entity> items = null;
            try
            {
                items = await (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Empty)).ToListAsync();
            }
            catch (Exception ex)
            {
                _log.WriteException(ex);
            }
            var result = items?.Select(entity => _mapper.Convert<Model>(entity));
            return result;
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
                result = _mapper.Convert<Model>(entity);
            }
            return result;
        }

        public virtual async Task<Model> CreateItemAsync(CreateModel model)
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
            Model result = default;
            if (entity != null)
            {
                result = _mapper.Convert<Model>(entity);
            }
            return result;
        }

        public virtual async Task<Model> UpdateItemAsync(UpdateModel model)
        {
            Entity entity = default;
            try
            {
                entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Single();
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
                var replaceResult = await _dbContext.Collection.ReplaceOneAsync(s => s.UserId == model.UserId, entity);
                if (replaceResult.IsAcknowledged && replaceResult.ModifiedCount == 1)
                {
                    entity = (await _dbContext.Collection.FindAsync(Builders<Entity>.Filter.Where(i => i.UserId == model.UserId))).Single();
                }
                else
                {
                    _log.WriteWarning($"Failed to update item. UserId: {model.UserId}");
                }
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
            return result;
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
    }
}