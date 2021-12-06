namespace FitnessApp.Abstractions.Models.Base
{
    public interface ISearchableModel : IModel
    {
        bool Matches(string search);
    }
}
