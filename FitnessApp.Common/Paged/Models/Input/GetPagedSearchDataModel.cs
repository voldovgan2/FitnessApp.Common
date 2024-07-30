using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Models.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedSearchDataModel : GetPagedDataModel
{
    public string Search { get; set; }
}
