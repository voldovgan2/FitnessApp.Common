using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged;
[ExcludeFromCodeCoverage]
public abstract class GetPagedDataContract
{
    public string SortBy { get; set; }
    public bool Asc { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class GetPagedSearchDataContract : GetPagedDataContract
{
    public string Search { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class GetPagedByIdsDataContract : GetPagedDataContract
{
    public string[] UserIds { get; set; }
}

[ExcludeFromCodeCoverage]
public class PagedDataContract<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public T[] Items { get; set; }
}
