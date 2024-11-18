using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Files;
using FitnessApp.Common.ServiceBus.Nats.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace FitnessApp.Common.Tests.Fixtures;

public class TestWebApplicationFactoryBase<TProgram, TAuthenticationHandler> : WebApplicationFactory<TProgram>
    where TProgram : class
    where TAuthenticationHandler : MockAuthenticationHandlerBase
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .ConfigureTestServices(services =>
            {
                services
                    .AddAuthentication(defaultScheme: MockConstants.Scheme)
                    .AddScheme<AuthenticationSchemeOptions, TAuthenticationHandler>(MockConstants.Scheme, options => { });

                services.RemoveAll<IServiceBus>();
                services.AddSingleton<IServiceBus, MockServiceBus>();
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

public class TestGenericWebApplicationFactoryBase<
    TProgram,
    TAuthenticationHandler,
    TEntity
    >(
    MongoDbFixtureBase<TEntity> fixture,
    string databaseName,
    string collecttionName,
    string[] ids) :
    TestWebApplicationFactoryBase<TProgram, TAuthenticationHandler>
    where TProgram : class
    where TAuthenticationHandler : MockAuthenticationHandlerBase
    where TEntity : IWithUserIdEntity
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<IMongoClient>();
                services.AddTransient<IMongoClient, MongoClient>((_) => fixture.Client);
            });
        fixture.SeedData(databaseName, collecttionName, ids).GetAwaiter().GetResult();
    }

    public Task SeedData(
        string databaseName,
        string collecttionName,
        string[] ids)
    {
        return fixture.SeedData(databaseName, collecttionName, ids);
    }
}

public class TestGenericFileAggregatorWebApplicationFactoryBase<
    TProgram,
    TAuthenticationHandler,
    TEntity
    >(
    MongoDbFixtureBase<TEntity> fixture,
    string databaseName,
    string collecttionName,
    string[] ids) :
    TestGenericWebApplicationFactoryBase<TProgram, TAuthenticationHandler, TEntity>(
        fixture,
        databaseName,
        collecttionName,
        ids)
    where TProgram : class
    where TAuthenticationHandler : MockAuthenticationHandlerBase
    where TEntity : IWithUserIdEntity
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<IFilesService>();
                services.AddSingleton<IFilesService, MockFilesService>();
            });
    }
}
