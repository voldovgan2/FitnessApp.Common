using FitnessApp.Abstractions.Db.Enums.Collection;
using FitnessApp.Abstractions.Models.Base;

namespace FitnessApp.Abstractions.Models.Collection
{
    public interface IUpdateCollectionModel : IUpdateModel
    {
        string CollectionName { get; set; }
        UpdateCollectionAction Action { get; set; }
        ICollectionItemModel Model { get; set; }
    }
}