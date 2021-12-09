using FitnessApp.Common.Paged.Models.Input;

namespace FitnessApp.Common.Abstractions.Models.Collection
{
    public class GetFilteredCollectionItemsModel : GetPagedDataModel
    {
        public string UserId { get; set; }
        public string CollectionName { get; set; }
    }
}
