using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;

namespace FitnessApp.Common.Abstractions.Models.GenericBlobAggregator
{
    public interface ICreateGenericBlobAggregatorModel
    {
        string UserId { get; set; }
        List<BlobImageModel> Images { get; set; }
    }
}
