using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Models.Collection
{
    public interface ICreateCollectionModel
    {
        string UserId { get; set; }
        Dictionary<string, IEnumerable<ICollectionItemModel>> Collection { get; set; }
    }
}
