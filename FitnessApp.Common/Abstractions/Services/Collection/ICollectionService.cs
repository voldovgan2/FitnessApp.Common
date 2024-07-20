using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.Collection;

public interface ICollectionService<
    TCollectionModel,
    TCollectionItemModel,
    TCreateCollectionModel,
    TUpdateCollectionModel>
    where TCollectionModel : ICollectionModel
    where TCollectionItemModel : ICollectionItemModel
    where TCreateCollectionModel : ICreateCollectionModel
    where TUpdateCollectionModel : IUpdateCollectionModel
{
    Task<TCollectionModel> GetItemByUserId(string userId);
    Task<IEnumerable<TCollectionItemModel>> GetCollectionByUserId(string userId, string collectionName);
    Task<PagedDataModel<TCollectionItemModel>> GetFilteredCollectionItems(string search, GetFilteredCollectionItemsModel<TCollectionItemModel> model);
    Task<string> CreateItem(TCreateCollectionModel model);
    Task<TCollectionItemModel> UpdateItem(TUpdateCollectionModel model);
    Task<TCollectionModel> DeleteItem(string userId);
}