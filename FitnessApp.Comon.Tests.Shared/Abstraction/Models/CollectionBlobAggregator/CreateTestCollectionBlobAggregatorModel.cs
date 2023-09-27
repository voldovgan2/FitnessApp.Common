using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator
{
    public class CreateTestCollectionBlobAggregatorModel : ICreateCollectionBlobAggregatorModel
    {
        public string UserId { get; set; }
        public Dictionary<string, IEnumerable<ICollectionItemModel>> Collection { get; set; }
    }
}
