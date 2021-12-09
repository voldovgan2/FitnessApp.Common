using FitnessApp.Common.Abstractions.Db.Entities.Base;
using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Db.Entities.Collection
{
    public interface ICollectionEntity : IEntity
    {
        Dictionary<string, List<ICollectionItemEntity>> Collection { get; set; }
    }
}