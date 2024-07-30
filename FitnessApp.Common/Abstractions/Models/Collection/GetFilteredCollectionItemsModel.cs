using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Paged.Models.Input;

namespace FitnessApp.Common.Abstractions.Models.Collection;

[ExcludeFromCodeCoverage]
public abstract class GetFilteredCollectionItemsModel : GetPagedSearchDataModel
{
    public string UserId { get; set; }
    public string CollectionName { get; set; }
}
