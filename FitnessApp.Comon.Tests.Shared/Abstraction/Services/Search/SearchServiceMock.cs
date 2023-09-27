using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Search;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.Search
{
    public class SearchServiceMock : ISearchService
    {
        public Task<IEnumerable<string>> Search(string search)
        {
            return Task.FromResult(Enumerable.Empty<string>());
        }
    }
}
