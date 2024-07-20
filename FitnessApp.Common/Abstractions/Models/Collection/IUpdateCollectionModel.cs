using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.Collection;

public interface IUpdateCollectionModel : IUpdateGenericModel
{
    string CollectionName { get; set; }
    UpdateCollectionAction Action { get; set; }
    ICollectionItemModel Model { get; set; }
}