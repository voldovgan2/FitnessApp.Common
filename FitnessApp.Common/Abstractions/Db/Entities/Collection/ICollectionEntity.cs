using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;

namespace FitnessApp.Common.Abstractions.Db.Entities.Collection;

public interface ICollectionEntity : IGenericEntity
{
    Dictionary<string, List<ICollectionItemEntity>> Collection { get; set; }
}