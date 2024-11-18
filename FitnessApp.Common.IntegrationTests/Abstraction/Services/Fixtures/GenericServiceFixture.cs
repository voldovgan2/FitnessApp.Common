using System.Collections.Generic;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;

public abstract class GenericServiceFixture : DbContextFixtureBase<TestGenericEntity>
{
    protected GenericServiceFixture()
        : base((itemId) => TestData.CreateGenericEntity(new Dictionary<string, object>
            {
                {
                    "Id", itemId
                }
            }))
    {
    }
}