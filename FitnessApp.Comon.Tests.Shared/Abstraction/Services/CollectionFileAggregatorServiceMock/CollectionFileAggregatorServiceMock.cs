using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Abstractions.Services.CollectionFileAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionFileAggregatorServiceMock
{
    public class CollectionFileAggregatorServiceMock : CollectionFileAggregatorService<TestCollectionFileAggregatorModel, TestCollectionFileAggregatorItemModel, TestCollectionModel, TestCollectionItemModel, CreateTestCollectionFileAggregatorModel, CreateTestCollectionModel, UpdateTestCollectionFileAggregatorModel, UpdateTestCollectionModel>
    {
        public CollectionFileAggregatorServiceMock(
            ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel> collectionService,
            IFilesService filesService,
            IMapper mapper,
            CollectionFileAggregatorSettings collectionFileAggregatorSettings
        ) : base(collectionService, filesService, mapper, collectionFileAggregatorSettings) { }
    }
}
