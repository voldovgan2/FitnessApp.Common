using System.Collections.Generic;

namespace FitnessApp.Common.Paged.Models.Input;
public abstract class GetPagedByIdsDataModel : GetPagedDataModel
{
    public IEnumerable<string> Ids { get; set; }
}
