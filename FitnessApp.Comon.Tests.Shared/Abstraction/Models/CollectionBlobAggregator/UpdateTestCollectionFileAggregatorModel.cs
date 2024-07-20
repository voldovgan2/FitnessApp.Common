using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator;

public class UpdateTestCollectionFileAggregatorModel : IUpdateCollectionFileAggregatorModel
{
    public string UserId { get; set; }
    public string CollectionName { get; set; }
    public UpdateCollectionAction Action { get; set; }
    public ICollectionFileAggregatorItemModel<ICollectionItemModel> Model { get; set; }
}
