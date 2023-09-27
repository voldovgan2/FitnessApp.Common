using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Services.CollectionBlobAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Common.IntegrationTests.Blob.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionBlobAggregatorServiceMock;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Search;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.CollectionBlobAggregator
{
    [Collection("CollectionBlobAggregatorService collection")]
    public class CollectionBlobAggregatorServiceTest : IClassFixture<CollectionBlobAggregatorServiceCollectionServiceFixture>, IClassFixture<CollectionBlobAggregatorServiceBlobFixture>
    {
        private readonly ICollectionBlobAggregatorService<TestCollectionBlobAggregatorModel, TestCollectionBlobAggregatorItemModel, TestCollectionItemModel, CreateTestCollectionBlobAggregatorModel, UpdateTestCollectionBlobAggregatorModel> _service;

        public CollectionBlobAggregatorServiceTest(CollectionBlobAggregatorServiceCollectionServiceFixture dbContextFixture, CollectionBlobAggregatorServiceBlobFixture blobFixture)
        {
            var collectionBlobAggregatorSettings = new CollectionBlobAggregatorSettings
            {
                ContainerName = blobFixture.Path,
                CollectionsBlobFields = new Dictionary<string, string[]>
                {
                    {
                        TestData.CollectionName,
                        new string[]
                        {
                            TestData.BlobFieldName
                        }
                    }
                }
            };
            var searchMock = new SearchServiceMock();
            _service = new CollectionBlobAggregatorServiceMock(
                new CollectionServiceMock(
                    new CollectionRepositoryMock(dbContextFixture.DbContext, dbContextFixture.Mapper),
                    searchMock
                ),
                blobFixture.BlobService,
                dbContextFixture.Mapper,
                collectionBlobAggregatorSettings
            );
        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem()
        {
            // Arrange
            var getFilteredCollectionItemsModel = new GetTestFilteredCollectionItemsModel
            {
                UserId = TestData.EntityIdToGet,
                Predicate = i => i.TestProperty.Contains("Property"),
                CollectionName = TestData.CollectionName,
                Page = 0,
                PageSize = 10
            };

            // Act
            var testCollectionBlobAggregatorItemModel = await _service.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.Collection(
                testCollectionBlobAggregatorItemModel.Items,
                item => Assert.NotNull(item.Images.Single().FieldName),
                item => Assert.NotNull(item.Images.Single().Value)
            );
        }

        [Fact]
        public async Task GetFilteredCollectionItems_ReturnsItemsWithBlobs()
        {
            // Arrange
            var getFilteredCollectionItemsModel = new GetTestFilteredCollectionItemsModel
            {
                UserId = TestData.EntityIdToGet,
                Predicate = i => true,
                CollectionName = TestData.CollectionName,
                Page = 0,
                PageSize = 10
            };

            // Act
            var testCollectionBlobAggregatorItemModel = await _service.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.Collection(
                testCollectionBlobAggregatorItemModel.Items,
                item => Assert.NotNull(item.Images.Single().FieldName),
                item => Assert.NotNull(item.Images.Single().Value)
            );
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Arrange
            var createCollectionBlobAggregatorModel = TestData.CreateCreateTestCollectionBlobAggregatorModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.EntityIdToCreate
                    }
                }
            );

            // Act
            var id = await _service.CreateItem(createCollectionBlobAggregatorModel);

            // Assert
            Assert.True(id == TestData.EntityIdToCreate);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Add
                },
                {
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 3
                        }
                    })
                }
            });

            // Act
            var testCollectionBlobAggregatorItemModel = await _service.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
        {
            // Arrange
            await _service.UpdateItem(TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Add
                },
                {
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 4
                        }
                    })
                }
            }));

            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Update
                },
                {
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 4
                        }
                    })
                }
            });

            // Act
            var testCollectionBlobAggregatorItemModel = await _service.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemRemoveCollectionItem_ReturnsRemovedItem()
        {
            // Arrange
            await _service.UpdateItem(TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Add
                },
                {
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 5
                        }
                    })
                }
            }));

            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Remove
                },
                {
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 5
                        }
                    })
                }
            });

            // Act
            var testCollectionBlobAggregatorItemModel = await _service.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionBlobAggregatorItemModel.Images.Single().Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItem()
        {
            // Act
            var deletedId = await _service.DeleteItem(TestData.EntityIdToDelete);

            // Assert
            Assert.True(deletedId == TestData.EntityIdToDelete);
        }
    }
}
