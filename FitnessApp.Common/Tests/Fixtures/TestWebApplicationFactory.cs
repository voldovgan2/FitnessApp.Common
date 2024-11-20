using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Tests.Fixtures;

public class TestWebApplicationFactory<
    TProgram,
    TAuthenticationHandler,
    TEntity>(MongoDbFixture<TEntity> fixture) :
    WebApplicationFactory<TProgram>
    where TProgram : class
    where TAuthenticationHandler : MockAuthenticationHandlerBase
    where TEntity : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        builder
            .ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication(defaultScheme: MockConstants.Scheme)
                    .AddScheme<AuthenticationSchemeOptions, TAuthenticationHandler>(MockConstants.Scheme, options => { });
            })
            .UseEnvironment("Development");
    }

    public HttpClient CreateHttpClient()
    {
        var httpClient = CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: MockConstants.Scheme);
        return httpClient;
    }
}
