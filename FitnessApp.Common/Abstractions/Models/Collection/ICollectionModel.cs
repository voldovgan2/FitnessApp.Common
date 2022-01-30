using FitnessApp.Common.Abstractions.Models.Base;
using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Models.Collection
{
    public interface ICollectionModel : IModel
    {
        string UserId { get; set; }
        Dictionary<string, List<ICollectionItemModel>> Collection { get; set; }
    }
}