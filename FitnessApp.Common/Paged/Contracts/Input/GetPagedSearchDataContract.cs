namespace FitnessApp.Common.Paged.Contracts.Input;
public abstract class GetPagedSearchDataContract : GetPagedDataContract
{
    public string Search { get; set; }
}
