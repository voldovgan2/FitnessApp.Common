using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Models.GenericFileAggregator
{
        public interface IGenericFileAggregatorModel<TGenericModel>
            where TGenericModel : IGenericModel
    {
        public TGenericModel Model { get; set; }
        public List<FileImageModel> Images { get; set; }
    }
}
