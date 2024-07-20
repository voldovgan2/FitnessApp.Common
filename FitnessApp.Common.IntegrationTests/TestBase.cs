using System.IO;
using AutoMapper;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Common.IntegrationTests;

public class TestBase
{
    public IMapper Mapper { get; }
    public IConfiguration Configuration { get; }

    public TestBase()
    {
        Configuration = CreateConfiguration();

        Mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }).CreateMapper();
    }

    private static IConfiguration CreateConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        return configurationBuilder.Build();
    }
}