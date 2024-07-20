using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;

public class CollectionRepositoryMockCollectionRepositoryMock(IDbContext<TestCollectionEntity> dbContext, IMapper mapper) :
    CollectionRepository<
        TestCollectionEntity,
        TestCollectionItemEntity,
        TestCollectionModel,
        TestCollectionItemModel,
        CreateTestCollectionModel,
        UpdateTestCollectionModel>(dbContext, mapper);
