namespace FitnessApp.Common.Paged.Contracts.Input
{
    public abstract class GetPagedDataContract
    {
        public string Search { get; set; }
        public string SortBy { get; set; }
        public bool Asc { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
