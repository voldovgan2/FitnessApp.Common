using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;

namespace FitnessApp.Common.Abstractions.Models.GenericBlobAggregator
{
    public interface IUpdateGenericBlobAggregatorModel
    {
        string UserId { get; set; }
        List<BlobImageModel> Images { get; set; }
    }
}