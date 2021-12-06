using FitnessApp.Abstractions.Db.Entities.Base;
using System.Collections.Generic;

namespace FitnessApp.Abstractions.Db.Entities.Collection
{
    public interface ICollectionEntity : IEntity
    {
        Dictionary<string, List<ICollectionItemEntity>> Collection { get; set; }
    }
}