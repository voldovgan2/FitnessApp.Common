namespace FitnessApp.Common.Paged.Models.Input;

public abstract class GetPagedDataModel
{
    public string Search { get; set; }
    public string SortBy { get; set; }
    public bool Asc { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
