using FitnessApp.Abstractions.Db.Entities.Base;
using FitnessApp.Abstractions.Models.Base;
using FitnessApp.Abstractions.Db.Repository.Base;
using FitnessApp.Abstractions.Services.Cache;
using FitnessApp.Logger;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace FitnessApp.Abstractions.Services.Base
{
    public class GenericService<Entity, Model, GetItemsModel, CreateModel, UpdateModel>
        : IGenericService<Entity, Model, GetItemsModel, CreateModel, UpdateModel>
        where Entity : IEntity
        where Model : ISearchableModel
        where GetItemsModel : IGetItemsModel
        where CreateModel : ICreateModel
        where UpdateModel : IUpdateModel
    {
        private readonly IGenericRepository<Entity, Model, CreateModel, UpdateModel> _repository;
        private readonly ICacheService<Model> _cacheService;
        private readonly ILogger<GenericService<Entity, Model, GetItemsModel, CreateModel, UpdateModel>> _log;

        public GenericService
        (
            IGenericRepository<Entity, Model, CreateModel, UpdateModel> repository,
            ICacheService<Model> cacheService,            
            ILogger<GenericService<Entity, Model, GetItemsModel, CreateModel, UpdateModel>> log
        )
        {
            _repository = repository;
            _cacheService = cacheService;
            _log = log;
        }

        public virtual async Task<Model> GetItemByUserIdAsync(string userId)
        {
            Model result = await _cacheService.GetItem(userId);
            if (result == null)
            {
                result = await _repository.GetItemByUserIdAsync(userId);
                if (result != null)
                {
                    await _cacheService.SaveItem(userId, result);
                }
                else
                {
                    _log.WriteWarning($"Failed to get item from database. Item: {userId}");
                }
            }
            return result;
        }

        public virtual async Task<IEnumerable<Model>> GetItemsAsync(GetItemsModel model)
        {
            IEnumerable<Model> result = null;
            var cachedItems = new List<Model>();
            foreach (var key in model?.UsersIds)
            {
                var item = await _cacheService.GetItem(key);
                if (item != null)
                {
                    cachedItems.Add(item);
                }
            }
            if (cachedItems.Any())
            {
                result = cachedItems;
            }
            if (result == null)
            {
                var items = await _repository.GetAllItemsAsync();
                if (items != null)
                {
                    result = items;
                    foreach (var item in items)
                    {
                        await _cacheService.SaveItem(item.UserId, item);
                    }
                }
                else
                {
                    _log.WriteWarning($"Failed to get items from database.");
                }
            }
            if (result != null)
            {
                if (!string.IsNullOrWhiteSpace(model.Search))
                {
                    result = result
                        .Where(p => p.Matches(model.Search));
                }
                if (model.UsersIds?.Any() == true)
                {
                    result = result.Where(p => model.UsersIds.Contains(p.UserId));
                }
            }
            return result;
        }

        public virtual async Task<Model> CreateItemAsync(CreateModel model)
        {
            var result = await _repository.CreateItemAsync(model);
            if (result != null)
            {
                await _cacheService.SaveItem(model.UserId, result);
            }
            return result;
        }

        public virtual async Task<Model> UpdateItemAsync(UpdateModel model)
        {
            var result = await _repository.UpdateItemAsync(model);
            if (result != null)
            {
                await _cacheService.SaveItem(model.UserId, result);
            }
            return result;
        }

        public virtual async Task<string> DeleteItemAsync(string userId)
        {
            string result = await _repository.DeleteItemAsync(userId);
            if (result != null)
            {
                await _cacheService.DeleteItem(result);
            }
            return result;
        }
    }
}