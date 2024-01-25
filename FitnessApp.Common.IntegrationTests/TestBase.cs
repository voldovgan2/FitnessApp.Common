using System.IO;
using AutoMapper;
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
#if RELEASE
                .AddJsonFile("appsettings.json");
#endif
#if DEBUG
                .AddJsonFile("appsettings.development.json");
#endif
            Configuration = configurationBuilder
              .Build();

            Mapper = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<MappingProfile>();
                }).CreateMapper();
        }
    }
}