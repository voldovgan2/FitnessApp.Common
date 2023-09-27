using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator
{
    public class TestCollectionBlobAggregatorModel : ICollectionBlobAggregatorModel
    {
        public string UserId { get; set; }
        public Dictionary<string, List<ICollectionBlobAggregatorItemModel<ICollectionItemModel>>> Collection { get; set; }
    }
}
