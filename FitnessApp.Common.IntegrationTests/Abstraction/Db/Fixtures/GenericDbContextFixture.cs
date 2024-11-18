using System.Collections.Generic;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;

public abstract class GenericDbContextFixture : DbContextFixtureBase<TestGenericEntity>
{
    protected GenericDbContextFixture()
        : base((itemId) => TestData.CreateGenericEntity(new Dictionary<string, object>
            {
                {
                    "Id", itemId
                }
            }))
    {
    }
}
