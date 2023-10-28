using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator
{
    public class TestCollectionFileAggregatorModel : ICollectionFileAggregatorModel
    {
        public string UserId { get; set; }
        public Dictionary<string, List<ICollectionFileAggregatorItemModel<ICollectionItemModel>>> Collection { get; set; }
    }
}
