using System.Collections.Generic;

namespace FitnessApp.Common.Paged.Models.Output;

public class PagedDataModel<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; }
}
