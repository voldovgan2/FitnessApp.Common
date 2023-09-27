using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection
{
    public class CollectionRepositoryMock : CollectionRepository<TestCollectionEntity, TestCollectionItemEntity, TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>
    {
        public CollectionRepositoryMock(IDbContext<TestCollectionEntity> dbContext, IMapper mapper)
            : base(dbContext, mapper) { }
    }
}
