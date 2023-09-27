using System.Collections.Generic;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures
{
    public class GenericBlobAggregatorServiceGenericServiceFixture : DbContextFixtureBase<TestGenericEntity>
    {
        public GenericBlobAggregatorServiceGenericServiceFixture()
            : base("GenericBlobAggregatorService", (itemId) => TestData.CreateGenericEntity(new Dictionary<string, object>
                {
                    {
                        "Id", itemId
                    }
                }))
        {
        }
    }
}