using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;

[ExcludeFromCodeCoverageAttribute]
public class CollectionServiceMock(
        ICollectionRepository<
            TestCollectionModel,
            TestCollectionItemModel,
            CreateTestCollectionModel,
            UpdateTestCollectionModel> repository
        ) :
    CollectionService<
        TestCollectionModel,
        TestCollectionItemModel,
        CreateTestCollectionModel,
        UpdateTestCollectionModel>(repository);
