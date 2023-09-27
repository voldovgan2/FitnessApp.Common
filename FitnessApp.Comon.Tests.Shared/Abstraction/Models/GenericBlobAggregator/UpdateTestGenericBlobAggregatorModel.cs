using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator
{
    public class UpdateTestGenericBlobAggregatorModel : IUpdateGenericBlobAggregatorModel
    {
        public string UserId { get; set; }
        public List<BlobImageModel> Images { get; set; }
        public string TestProperty { get; set; }
    }
}
