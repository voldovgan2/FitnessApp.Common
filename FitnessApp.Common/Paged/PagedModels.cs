using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged;

[ExcludeFromCodeCoverage]
public abstract class GetPagedDataModel
{
    public string SortBy { get; set; }
    public bool Asc { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class GetPagedSearchDataModel : GetPagedDataModel
{
    public string Search { get; set; }
}

[ExcludeFromCodeCoverage]
public class GetPagedByIdDataModel : GetPagedSearchDataModel
{
    public string Id { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class GetPagedByIdsDataModel : GetPagedDataModel
{
    public string[] UserIds { get; set; }
}

[ExcludeFromCodeCoverage]
public class PagedDataModel<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public T[] Items { get; set; }
}
