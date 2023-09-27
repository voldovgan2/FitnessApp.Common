using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.GenericBlobAggregator
{
        public interface IGenericBlobAggregatorModel<TGenericModel>
            where TGenericModel : IGenericModel
    {
        public TGenericModel Model { get; set; }
        public List<BlobImageModel> Images { get; set; }
    }
}
