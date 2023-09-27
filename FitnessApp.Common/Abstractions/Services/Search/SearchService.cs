using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Services.Search.SearchServiceClient;
using FitnessApp.Common.Attributes;

namespace FitnessApp.Common.Abstractions.Services.Search
{
    public class SearchService<TGenericEntity> : ISearchService
        where TGenericEntity : IGenericEntity
    {
        private readonly ISearchServiceClient _searchClient;
        public SearchService(ISearchServiceClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task<IEnumerable<string>> Search(string search)
        {
            var response = await _searchClient.SearchAsync<SearchDocument>(GetQuery(search), new SearchOptions { Size = 5 });
            return response == null ?
                Enumerable.Empty<string>()
                : response.Value.GetResults().Select(r => r.Document["doc_id"] as string);
        }

        private string GetQuery(string search)
        {
            var singleWordSearchableProperties = typeof(TGenericEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(SingleWordSearchableAttribute), false).Any());

            var multiWordSearchableProperties = typeof(TGenericEntity)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(MultiWordSearchableAttribute), false).Any());

            var allProperties = Enumerable.Concat(singleWordSearchableProperties, multiWordSearchableProperties);

            string result;
            var searchWords = search.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (searchWords.Length == 1)
            {
                result = $"$filter={WrapIsMatchFunction(CreateSingleWordCondition(allProperties.Select(p => p.Name), search))}";
            }
            else
            {
                var allConditions = new List<string>();
                foreach (var word in searchWords)
                {
                    allConditions.Add(WrapIsMatchFunction(CreateSingleWordCondition(singleWordSearchableProperties.Select(p => p.Name), word)));
                }

                allConditions.Add(WrapIsMatchFunction(CreateSingleWordCondition(multiWordSearchableProperties.Select(p => p.Name), search)));
                result = $"$filter={string.Join(" or ", allConditions)}";
            }

            return result;
        }

        private string CreateSingleWordCondition(IEnumerable<string> fields, string search)
        {
            return $"'*{search}*','{string.Join(',', fields)}'";
        }

        private string WrapIsMatchFunction(string body)
        {
            return $"search.ismatch({body})";
        }
    }
}
