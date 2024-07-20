using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

public class UpdateTestGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
{
    public string UserId { get; set; }
    public List<FileImageModel> Images { get; set; }
    public string TestProperty { get; set; }
}
