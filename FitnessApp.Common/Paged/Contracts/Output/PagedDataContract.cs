using System.Collections.Generic;

namespace FitnessApp.Common.Paged.Contracts.Output;

public class PagedDataContract<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; }
}
