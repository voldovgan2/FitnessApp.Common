namespace FitnessApp.Common.Abstractions.Models.Collection;
public interface ICollectionItemWithNameAndDescriptionModel : ICollectionItemModel
{
    public string Name { get; set; }
    public string Description { get; set; }
}
