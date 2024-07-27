using System.Collections.Generic;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;

public class GenericFileAggregatorServiceGenericServiceFixture : DbContextFixtureBase<TestGenericEntity>
{
    public GenericFileAggregatorServiceGenericServiceFixture()
        : base((itemId) => TestData.CreateGenericEntity(new Dictionary<string, object>
            {
                {
                    "Id", itemId
                }
            }))
    {
    }
}