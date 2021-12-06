namespace FitnessApp.Abstractions.Models.Collection
{
    public interface ISearchableCollectionItemModel : ICollectionItemModel
    {
        bool Matches(string search);
    }
}
