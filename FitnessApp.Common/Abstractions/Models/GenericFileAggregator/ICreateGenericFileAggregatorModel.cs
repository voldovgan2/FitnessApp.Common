using FitnessApp.Common.Abstractions.Models.FileImage;

namespace FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

public interface ICreateGenericFileAggregatorModel
{
    string UserId { get; set; }
    FileImageModel[] Images { get; set; }
}
