using FitnessApp.Paged.Models.Input;

namespace FitnessApp.Abstractions.Models.Collection
{
    public class GetFilteredCollectionItemsModel : GetPagedDataModel
    {
        public string UserId { get; set; }
        public string CollectionName { get; set; }
    }
}
