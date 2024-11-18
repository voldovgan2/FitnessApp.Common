using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Models.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedByIdsDataModel : GetPagedDataModel
{
    public string[] UserIds { get; set; }
}
