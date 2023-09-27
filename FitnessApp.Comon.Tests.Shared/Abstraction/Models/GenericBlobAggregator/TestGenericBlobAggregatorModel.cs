using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator
{
    public class TestGenericBlobAggregatorModel : IGenericBlobAggregatorModel<TestGenericModel>
    {
        public TestGenericModel Model { get; set; }
        public List<BlobImageModel> Images { get; set; }
    }
}
