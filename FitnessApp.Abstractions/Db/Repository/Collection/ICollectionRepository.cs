using FitnessApp.Abstractions.Db.Entities.Collection;
using FitnessApp.Abstractions.Models.Collection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Db.Repository.Collection
{
    public interface ICollectionRepository<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        where Entity : ICollectionEntity
        where CollectionItemEntity : ICollectionItemEntity
        where Model : ICollectionModel
        where CollectionItemModel : ISearchableCollectionItemModel
        where CreateModel : ICreateCollectionModel
        where UpdateModel : IUpdateCollectionModel
    {
        Task<Model> GetItemByUserIdAsync(string userId);
        Task<string> CreateItemAsync(CreateModel model);
        Task<CollectionItemModel> UpdateItemAsync(UpdateModel model);
        Task<string> UpdateItemsAsync(IEnumerable<UpdateModel> models);
        Task<string> DeleteItemAsync(string userId);
    }
}