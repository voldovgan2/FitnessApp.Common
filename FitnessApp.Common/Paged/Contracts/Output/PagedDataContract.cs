using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Contracts.Output;

[ExcludeFromCodeCoverage]
public class PagedDataContract<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public T[] Items { get; set; }
}
