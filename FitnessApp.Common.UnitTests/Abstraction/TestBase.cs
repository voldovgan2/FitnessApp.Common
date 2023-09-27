using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using MockQueryable.Moq;
using Moq;

namespace FitnessApp.Common.UnitTests.Abstraction
{
    public class TestBase
    {
        protected readonly IMapper _mapper;

        public TestBase()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            })
                .CreateMapper();
        }

        #region Abstract

        public Mock<IQueryable<T>> GetQueryableMock<T>(List<T> allEntities)
            where T : class
        {
            return allEntities.AsQueryable().BuildMock();
        }

        #endregion

        #region Generic initialization

        public Mock<IQueryable<TestGenericEntity>> GetQueryableGenericEntitiesMock()
        {
            return GetQueryableMock(TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>()));
        }

        #endregion

        #region Collection initialization

        public Mock<IQueryable<TestCollectionEntity>> GetQueryableCollectionEntitiesMock()
        {
            return GetQueryableMock(
                TestData.GetAll(
                    TestData.CreateCollectionEntity,
                    new Dictionary<string, object>
                    {
                        {
                            "ItemsCount", 2
                        }
                    }
                )
            );
        }

        public Mock<IQueryable<TestCollectionEntity>> GetQueryableCollectionEntitiesMock(List<TestCollectionEntity> allItems)
        {
            return GetQueryableMock(allItems);
        }

        #endregion
    }
}