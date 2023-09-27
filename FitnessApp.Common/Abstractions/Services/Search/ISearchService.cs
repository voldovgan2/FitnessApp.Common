using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Services.Search
{
    public interface ISearchService
    {
        Task<IEnumerable<string>> Search(string search);
    }
}
