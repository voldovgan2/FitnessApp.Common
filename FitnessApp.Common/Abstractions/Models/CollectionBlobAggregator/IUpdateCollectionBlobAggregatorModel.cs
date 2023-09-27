using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator
{
    public interface IUpdateCollectionBlobAggregatorModel
    {
        string UserId { get; set; }
        string CollectionName { get; set; }
        UpdateCollectionAction Action { get; set; }
        ICollectionBlobAggregatorItemModel<ICollectionItemModel> Model { get; set; }
    }
}
