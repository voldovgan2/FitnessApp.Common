using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.CollectionFileAggregator;

public interface ICollectionFileAggregatorService<
    TCollectionFileAggregatorModel,
    TCollectionFileAggregatorItemModel,
    TCreateCollectionFileAggregatorModel,
    TUpdateCollectionFileAggregatorModel>
    where TCollectionFileAggregatorModel : ICollectionFileAggregatorModel
    where TCollectionFileAggregatorItemModel : ICollectionFileAggregatorItemModel<ICollectionItemModel>
    where TCreateCollectionFileAggregatorModel : ICreateCollectionFileAggregatorModel
    where TUpdateCollectionFileAggregatorModel : IUpdateCollectionFileAggregatorModel
{
    Task<TCollectionFileAggregatorModel> GetItemByUserId(string userId);
    Task<IEnumerable<TCollectionFileAggregatorItemModel>> GetCollectionByUserId(string userId, string collectionName);
    Task<PagedDataModel<TCollectionFileAggregatorItemModel>> GetFilteredCollectionItems(GetFilteredCollectionItemsModel model);
    Task<string> CreateItem(TCreateCollectionFileAggregatorModel model);
    Task<TCollectionFileAggregatorItemModel> UpdateItem(TUpdateCollectionFileAggregatorModel model);
    Task<string> DeleteItem(string userId);
}
