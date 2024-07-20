using System;
using FitnessApp.Common.Paged.Models.Input;

namespace FitnessApp.Common.Abstractions.Models.Collection;

public abstract class GetFilteredCollectionItemsModel<TCollectionItemModel> : GetPagedDataModel
    where TCollectionItemModel : ICollectionItemModel
{
    public string UserId { get; set; }
    public string CollectionName { get; set; }
    public Func<TCollectionItemModel, bool> Predicate { get; set; }
}
