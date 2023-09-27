using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.CollectionBlobAggregator
{
    public interface ICollectionBlobAggregatorService<TCollectionBlobAggregatorModel, TCollectionBlobAggregatorItemModel, TCollectionItemModel, TCreateCollectionBlobAggregatorModel, TUpdateCollectionBlobAggregatorModel>
        where TCollectionBlobAggregatorModel : ICollectionBlobAggregatorModel
        where TCollectionBlobAggregatorItemModel : ICollectionBlobAggregatorItemModel<ICollectionItemModel>
        where TCollectionItemModel : ICollectionItemModel
        where TCreateCollectionBlobAggregatorModel : ICreateCollectionBlobAggregatorModel
        where TUpdateCollectionBlobAggregatorModel : IUpdateCollectionBlobAggregatorModel
    {
        Task<TCollectionBlobAggregatorModel> GetItemByUserId(string userId);
        Task<PagedDataModel<TCollectionBlobAggregatorItemModel>> GetFilteredCollectionItems(GetFilteredCollectionItemsModel<TCollectionItemModel> model);
        Task<string> CreateItem(TCreateCollectionBlobAggregatorModel model);
        Task<TCollectionBlobAggregatorItemModel> UpdateItem(TUpdateCollectionBlobAggregatorModel model);
        Task<string> DeleteItem(string userId);
    }
}
