using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.FileImage;

namespace FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

public interface ICreateGenericFileAggregatorModel
{
    string UserId { get; set; }
    List<FileImageModel> Images { get; set; }
}
