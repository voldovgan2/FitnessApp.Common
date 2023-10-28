using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.FileImage;

namespace FitnessApp.Common.Abstractions.Models.CollectionFileAggregator
{
    public interface ICollectionFileAggregatorItemModel<TCollectionItemModel>
        where TCollectionItemModel : ICollectionItemModel
    {
        public TCollectionItemModel Model { get; set; }
        List<FileImageModel> Images { get; set; }
    }
}
