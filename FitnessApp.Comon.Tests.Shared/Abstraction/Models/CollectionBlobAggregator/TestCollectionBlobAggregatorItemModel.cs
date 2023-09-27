using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator
{
    public class TestCollectionBlobAggregatorItemModel : ICollectionBlobAggregatorItemModel<ICollectionItemModel>
    {
        public ICollectionItemModel Model { get; set; }
        public List<BlobImageModel> Images { get; set; }
    }
}
