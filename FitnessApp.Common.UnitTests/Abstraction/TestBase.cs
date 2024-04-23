using System.Collections.Generic;
using AutoMapper;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;

namespace FitnessApp.Common.UnitTests.Abstraction
{
    public abstract class TestBase
    {
        protected readonly IMapper _mapper;

        protected TestBase()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            })
                .CreateMapper();
        }

        protected IEnumerable<TestGenericEntity> GetGenericEntitiesMock()
        {
            return TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>());
        }

        protected IEnumerable<TestCollectionEntity> GetCollectionEntitiesMock()
        {
            return TestData.GetAll(
                TestData.CreateCollectionEntity,
                new Dictionary<string, object>
                {
                    {
                        "ItemsCount", 2
                    }
                });
        }
    }
}