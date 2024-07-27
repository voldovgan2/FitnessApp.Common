using System.Collections.Generic;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;

public class GenericRepositoryDbContextFixture : DbContextFixtureBase<TestGenericEntity>
{
    public GenericRepositoryDbContextFixture()
        : base((itemId) => TestData.CreateGenericEntity(new Dictionary<string, object>
            {
                {
                    "Id", itemId
                }
            }))
    {
    }
}