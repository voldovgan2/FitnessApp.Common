using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Models.Base
{
    public interface IGetItemsModel
    {
        public IEnumerable<string> UsersIds { get; set; }
        public string Search { get; set; }
    }
}