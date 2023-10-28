using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.CollectionFileAggregator
{
    public interface ICollectionFileAggregatorModel : IGenericModel
    {
        Dictionary<string, List<ICollectionFileAggregatorItemModel<ICollectionItemModel>>> Collection { get; set; }
    }
}