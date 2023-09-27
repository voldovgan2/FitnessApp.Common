using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Db.Repository.Generic
{
    public abstract class GenericRepository<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel>
        : IGenericRepository<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel>
        where TGenericEntity : IGenericEntity
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
        where TUpdateGenericModel : IUpdateGenericModel
    {
        private readonly IDbContext<TGenericEntity> _dbContext;
        private readonly IMapper _mapper;

        protected GenericRepository(IDbContext<TGenericEntity> dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public virtual Task<IQueryable<TGenericEntity>> GetAllItems()
        {
            var result = _dbContext.GetAllItems();
            return Task.FromResult(result);
        }

        public virtual async Task<TGenericModel> GetItemByUserId(string userId)
        {
            var entity = await _dbContext.GetItemById(userId);
            var result = _mapper.Map<TGenericModel>(entity);
            return result;
        }

        public virtual async Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids)
        {
            var result = await _dbContext.GetItemsByIds(ids);
            return result;
        }

        public virtual async Task<TGenericModel> CreateItem(TCreateGenericModel model)
        {
            var entity = _mapper.Map<TGenericEntity>(model);
            var created = await _dbContext.CreateItem(entity);
            var result = _mapper.Map<TGenericModel>(created);
            return result;
        }

        public virtual async Task<TGenericModel> UpdateItem(TUpdateGenericModel model)
        {
            var entity = await _dbContext.GetItemById(model.UserId);
            var propertiesToUpdate = model
                .GetType()
                .GetProperties()
                .Where(p => p.GetValue(model, null) != null)
                .Where(p => p.Name != nameof(model.UserId));
            var entityProperties = entity.GetType().GetProperties();
            foreach (var propertyToUpdate in propertiesToUpdate)
            {
                var entityProperty = Array.Find(entityProperties, p => p.Name == propertyToUpdate.Name);
                entityProperty.SetValue(entity, propertyToUpdate.GetValue(model, null));
            }

            var replaced = await _dbContext.UpdateItem(entity);
            var result = _mapper.Map<TGenericModel>(replaced);
            return result;
        }

        public virtual async Task<string> DeleteItem(string userId)
        {
            var deleted = await _dbContext.DeleteItem(userId);
            string result = deleted.UserId;
            return result;
        }
    }
}