using System.IO;
using AutoMapper;
using Azure.Data.AppConfiguration;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Common.IntegrationTests
{
    public class TestBase
    {
        public ISearchService SearchService { get; }
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }

        public TestBase()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();

            Configuration = configurationBuilder
              .Build();

            var _client = new ConfigurationClient(configuration.GetConnectionString("AppConfig"));
            var settings = _client.GetConfigurationSettings(new SettingSelector());
            foreach (var setting in settings)
            {
                Configuration[setting.Key] = setting.Value;
            }

            Mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                }).CreateMapper();
        }
    }
}