using System.Collections.Generic;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures
{
    public class CollectionRepositoryDbContextFixture : DbContextFixtureBase<TestCollectionEntity>
    {
        public CollectionRepositoryDbContextFixture()
            : base("CollectionRepository", (itemId) => TestData.CreateCollectionEntity(new Dictionary<string, object>
                {
                    {
                        "Id", itemId
                    },
                    {
                        "ItemsCount", 2
                    }
                }))
        {
        }
    }
}
