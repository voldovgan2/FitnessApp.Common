using System.Collections.Generic;

namespace FitnessApp.Common.Paged.Contracts.Input;
public abstract class GetPagedByIdsDataContract : GetPagedDataContract
{
    public IEnumerable<string> Ids { get; set; }
}
