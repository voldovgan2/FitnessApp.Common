using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Paged.Contracts.Input;

[ExcludeFromCodeCoverage]
public abstract class GetPagedByIdsDataContract : GetPagedDataContract
{
    public string[] UserIds { get; set; }
}
