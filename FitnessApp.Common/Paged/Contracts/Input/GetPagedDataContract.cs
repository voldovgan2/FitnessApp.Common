using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Contracts.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedDataContract
{
    public string SortBy { get; set; }
    public bool Asc { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
