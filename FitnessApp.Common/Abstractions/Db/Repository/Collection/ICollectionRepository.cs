using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Common.Abstractions.Db.Repository.Collection
{
    public interface ICollectionRepository<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel>
        where TCollectionModel : ICollectionModel
        where TCollectionItemModel : ICollectionItemModel
        where TCreateCollectionModel : ICreateCollectionModel
        where TUpdateCollectionModel : IUpdateCollectionModel
    {
        Task<TCollectionModel> GetItemByUserId(string userId);
        Task<IEnumerable<TCollectionItemModel>> GetCollectionByUserId(string userId, string collectionName);
        Task<string> CreateItem(TCreateCollectionModel model);
        Task<TCollectionItemModel> UpdateItem(TUpdateCollectionModel model);
        Task UpdateItems(IEnumerable<TUpdateCollectionModel> models);
        Task<TCollectionModel> DeleteItem(string userId);
    }
}