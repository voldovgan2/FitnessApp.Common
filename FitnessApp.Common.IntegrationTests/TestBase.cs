using System.IO;
using AutoMapper;
using Azure.Data.AppConfiguration;
using FitnessApp.Comon.Tests.Shared;
using Microsoft.Extensions.Configuration;

namespace FitnessApp.Common.IntegrationTests
{
    public class TestBase
    {
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }

        public TestBase()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = configurationBuilder
              .Build();

            Mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                }).CreateMapper();
        }
    }
}