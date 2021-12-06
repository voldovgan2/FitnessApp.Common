using System.Collections.Generic;

namespace FitnessApp.Abstractions.Models.Base
{
    public interface IGetItemsModel
    {
        public IEnumerable<string> UsersIds { get; set; }
        public string Search { get; set; }
    }
}