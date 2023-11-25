using System.Collections.Generic;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures
{
    public class CollectionFileAggregatorServiceCollectionServiceFixture : DbContextFixtureBase<TestCollectionEntity>
    {
        public CollectionFileAggregatorServiceCollectionServiceFixture()
            : base("CollectionFileAggregatorService", (itemId) => TestData.CreateCollectionEntity(new Dictionary<string, object>
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
