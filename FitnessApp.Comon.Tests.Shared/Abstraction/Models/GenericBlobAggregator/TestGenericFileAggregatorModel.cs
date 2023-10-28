using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator
{
    public class TestGenericFileAggregatorModel : IGenericFileAggregatorModel<TestGenericModel>
    {
        public TestGenericModel Model { get; set; }
        public List<FileImageModel> Images { get; set; }
    }
}
