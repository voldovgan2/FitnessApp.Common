using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Base;

namespace FitnessApp.Common.Abstractions.Models.Collection
{
    public interface IUpdateCollectionModel : IUpdateModel
    {
        string CollectionName { get; set; }
        UpdateCollectionAction Action { get; set; }
        ICollectionItemModel Model { get; set; }
    }
}