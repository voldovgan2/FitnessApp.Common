using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Contracts.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedByIdsDataContract : GetPagedDataContract
{
    public IEnumerable<string> Ids { get; set; }
}
