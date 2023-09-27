using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator
{
    public interface ICollectionBlobAggregatorItemModel<TCollectionItemModel>
        where TCollectionItemModel : ICollectionItemModel
    {
        public TCollectionItemModel Model { get; set; }
        List<BlobImageModel> Images { get; set; }
    }
}
