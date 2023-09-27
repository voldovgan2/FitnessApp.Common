using AutoMapper;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Abstractions.Services.CollectionBlobAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Blob;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionBlobAggregatorServiceMock
{
    public class CollectionBlobAggregatorServiceMock : CollectionBlobAggregatorService<TestCollectionBlobAggregatorModel, TestCollectionBlobAggregatorItemModel, TestCollectionModel, TestCollectionItemModel, CreateTestCollectionBlobAggregatorModel, CreateTestCollectionModel, UpdateTestCollectionBlobAggregatorModel, UpdateTestCollectionModel>
    {
        public CollectionBlobAggregatorServiceMock(
            ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel> collectionService,
            IBlobService blobService,
            IMapper mapper,
            CollectionBlobAggregatorSettings collectionBlobAggregatorSettings
        ) : base(collectionService, blobService, mapper, collectionBlobAggregatorSettings) { }
    }
}
