using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;

public class CollectionServiceMock : CollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>
{
    public CollectionServiceMock(
        ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel> repository
        ) : base(repository) { }
}
