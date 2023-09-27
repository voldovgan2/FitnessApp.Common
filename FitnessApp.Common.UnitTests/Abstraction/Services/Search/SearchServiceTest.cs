using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Common.Abstractions.Services.Search.SearchServiceClient;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.Search
{
    public class SearchServiceTest
    {
        [Theory]

        [InlineData("SingleWord", "$filter=search.ismatch('*SingleWord*','TestProperty1,TestProperty2,TestProperty3,TestProperty4')")]
        [InlineData("Multy Word", "$filter=search.ismatch('*Multy*','TestProperty1,TestProperty2') or search.ismatch('*Word*','TestProperty1,TestProperty2') or search.ismatch('*Multy Word*','TestProperty3,TestProperty4')")]
        public async Task SearchData_ValuePassed_BuildsCorrectQuery(string passed, string expected)
        {
            // Arrange
            var searchServiceClient = new Mock<ISearchServiceClient>();
            string actual = null;
            searchServiceClient
                .Setup(s => s.SearchAsync<SearchDocument>(It.IsAny<string>(), It.IsAny<SearchOptions>(), default))
                .Callback((string s, SearchOptions so, CancellationToken ct) => { actual = s; });

            var searchService = new SearchService<TestGenericEntity>(searchServiceClient.Object);

            // Act
            await searchService.Search(passed);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
