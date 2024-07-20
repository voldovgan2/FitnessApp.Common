using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;

public interface IUpdateCollectionFileAggregatorModel
{
    string UserId { get; set; }
    string CollectionName { get; set; }
    UpdateCollectionAction Action { get; set; }
    ICollectionFileAggregatorItemModel<ICollectionItemModel> Model { get; set; }
}
