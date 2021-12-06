using FitnessApp.Abstractions.Models.Base;
using System.Collections.Generic;

namespace FitnessApp.Abstractions.Models.Collection
{
    public interface ICollectionModel : IModel
    {
        string UserId { get; set; }
        Dictionary<string, IEnumerable<ICollectionItemModel>> Collection { get; set; }
    }
}