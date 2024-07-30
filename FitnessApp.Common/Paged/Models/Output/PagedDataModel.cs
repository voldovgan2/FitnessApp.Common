using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Models.Output;

[ExcludeFromCodeCoverage]
public class PagedDataModel<T>
{
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T> Items { get; set; }
}
