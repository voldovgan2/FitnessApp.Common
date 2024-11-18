using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

[ExcludeFromCodeCoverageAttribute]
public class CreateTestGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
{
    public string UserId { get; set; }
    public FileImageModel[] Images { get; set; }
    public string TestProperty { get; set; }
}
