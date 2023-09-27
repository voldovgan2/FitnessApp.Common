using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace FitnessApp.Common.Abstractions.Services.Search.SearchServiceClient
{
    public interface ISearchServiceClient
    {
        Task<Response<SearchResults<T>>> SearchAsync<T>(string searchText, SearchOptions options = null, CancellationToken cancellationToken = default);
    }
}
