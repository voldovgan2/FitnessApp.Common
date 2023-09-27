using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator
{
    public interface ICollectionBlobAggregatorModel : IGenericModel
    {
        Dictionary<string, List<ICollectionBlobAggregatorItemModel<ICollectionItemModel>>> Collection { get; set; }
    }
}