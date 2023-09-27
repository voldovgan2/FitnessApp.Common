using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace FitnessApp.Common.Abstractions.Services.Search.SearchServiceClient
{
    public class SearchServiceClient : ISearchServiceClient
    {
        private readonly SearchClient _searchClient;
        public SearchServiceClient(SearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public Task<Response<SearchResults<T>>> SearchAsync<T>(string searchText, SearchOptions options = null, CancellationToken cancellationToken = default)
        {
            return _searchClient.SearchAsync<T>(searchText, options, cancellationToken);
        }
    }
}
