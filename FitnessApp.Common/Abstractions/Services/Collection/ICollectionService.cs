using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Paged.Models.Output;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Services.Collection
{
    public interface ICollectionService<Entity, CollectionItemEntity, Model, CollectionItemModel, CreateModel, UpdateModel>
        where Entity : ICollectionEntity
        where CollectionItemEntity : ICollectionItemEntity
        where Model : ICollectionModel
        where CollectionItemModel : ISearchableCollectionItemModel
        where CreateModel : ICreateCollectionModel
        where UpdateModel : IUpdateCollectionModel
    {
        string DefaultCollectionName { get; }
        Task<Model> GetItemByUserIdAsync(string userId);
        Task<PagedDataModel<CollectionItemModel>> GetFilteredCollectionItemsAsync(GetFilteredCollectionItemsModel model);
        Task<string> CreateItemAsync(CreateModel model);
        Task<CollectionItemModel> UpdateItemAsync(UpdateModel model);
        Task<string> DeleteItemAsync(string userId);
    }
}