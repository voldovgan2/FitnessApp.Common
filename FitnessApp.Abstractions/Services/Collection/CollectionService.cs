using FitnessApp.Abstractions.Db.Entities.Collection;
using FitnessApp.Abstractions.Db.Repository.Collection;
using FitnessApp.Abstractions.Models.Collection;
using FitnessApp.Paged.Extensions;
using FitnessApp.Paged.Models.Output;
using FitnessApp.Serializer.JsonMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Services.Collection
{
    public class CollectionService<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        : ICollectionService<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        where Entity : ICollectionEntity
        where CollectionItemEntity : ICollectionItemEntity
        where Model : ICollectionModel
        where CollectionItemModel : ISearchableCollectionItemModel
        where CreateModel : ICreateCollectionModel
        where UpdateModel : IUpdateCollectionModel
    {
        private readonly ICollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel> _repository;
        private readonly IJsonMapper _mapper;

        public CollectionService
        (
            ICollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel> repository,
            IJsonMapper mapper,
            ILogger<CollectionService<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>> log
        )
        {
            _repository = repository;
            _mapper = mapper;            
        }

        public string DefaultCollectionName { get; protected set; }

        public virtual async Task<Model> GetItemByUserIdAsync(string userId)
        {
            var result = await _repository.GetItemByUserIdAsync(userId);            
            return result;
        }

        public virtual async Task<PagedDataModel<CollectionItemModel>> GetFilteredCollectionItemsAsync(GetFilteredCollectionItemsModel model)
        {
            PagedDataModel<CollectionItemModel> result = null;
            var item = await GetItemByUserIdAsync(model.UserId);
            if (item != null)
            {
                var allItems = _mapper.Convert<IEnumerable<CollectionItemModel>>(item.Collection[model.CollectionName]);
                if (allItems != null)
                {
                    allItems = allItems.Where(p => p.Matches(model.Search));
                    result = allItems.ToPaged(model);
                }
            }
            return result;
        }

        public virtual async Task<string> CreateItemAsync(CreateModel model)
        {
            model.Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>
            {
                { DefaultCollectionName, new List<ICollectionItemModel>() }
            };
            var result = await _repository.CreateItemAsync(model);            
            return result;
        }

        public virtual async Task<CollectionItemModel> UpdateItemAsync(UpdateModel model)
        {
            var result = await _repository.UpdateItemAsync(model);            
            return result;
        }

        public virtual async Task<string> DeleteItemAsync(string userId)
        {
            string result = await _repository.DeleteItemAsync(userId);            
            return result;
        }
    }
}