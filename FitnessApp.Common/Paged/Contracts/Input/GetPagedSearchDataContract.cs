using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Contracts.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedSearchDataContract : GetPagedDataContract
{
    public string Search { get; set; }
}
