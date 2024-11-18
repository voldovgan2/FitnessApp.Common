using FitnessApp.Common.Abstractions.Models.FileImage;

namespace FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

public interface IUpdateGenericFileAggregatorModel
{
    string UserId { get; set; }
    FileImageModel[] Images { get; set; }
}