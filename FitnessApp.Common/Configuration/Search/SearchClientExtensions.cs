using System;
using FitnessApp.Common.Abstractions.Services.Search.SearchServiceClient;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Configuration.Search
{
    public static class SearchClientExtensions
    {
        public static IServiceCollection ConfigureSearchService(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddAzureClients(builder =>
            {
                builder.AddSearchClient(configuration.GetSection("SearchClient"));
            });

            services.AddTransient<ISearchServiceClient, SearchServiceClient>();

            return services;
        }
    }
}
