using System.Collections.Generic;
using System.Linq;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Common.Abstractions.Services.Collection;
public abstract class CollectionServiceWithNameAndDescription<
    TCollectionModel,
    TCollectionItemModel,
    TCreateCollectionModel,
    TUpdateCollectionModel>(
        ICollectionRepository<TCollectionModel,
            TCollectionItemModel,
            TCreateCollectionModel,
            TUpdateCollectionModel> repository) :
    CollectionService<
        TCollectionModel,
        TCollectionItemModel,
        TCreateCollectionModel,
        TUpdateCollectionModel>(repository)
    where TCollectionModel : ICollectionModel
    where TCollectionItemModel : ICollectionItemWithNameAndDescriptionModel
    where TCreateCollectionModel : ICreateCollectionModel
    where TUpdateCollectionModel : IUpdateCollectionModel
{
    protected override IEnumerable<TCollectionItemModel> FilterItems(IEnumerable<TCollectionItemModel> items, string search)
    {
        return items.Where(i => i.Name.Contains(search)
            || i.Description.Contains(search));
    }
}
