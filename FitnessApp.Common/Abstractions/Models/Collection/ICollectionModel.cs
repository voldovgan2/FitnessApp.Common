using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.Collection
{
    public interface ICollectionModel : IGenericModel
    {
        Dictionary<string, List<ICollectionItemModel>> Collection { get; set; }
    }
}