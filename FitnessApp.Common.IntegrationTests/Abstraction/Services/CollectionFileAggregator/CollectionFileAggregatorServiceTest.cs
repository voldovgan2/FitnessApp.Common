using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Services.CollectionFileAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionFileAggregatorServiceMock;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.CollectionFileAggregator
{
    [Collection("CollectionFileAggregatorService collection")]
    public class CollectionFileAggregatorServiceTest : IClassFixture<CollectionFileAggregatorServiceCollectionServiceFixture>, IClassFixture<CollectionFileAggregatorServiceFileFixture>
    {
        private readonly ICollectionFileAggregatorService<TestCollectionFileAggregatorModel, TestCollectionFileAggregatorItemModel, TestCollectionItemModel, CreateTestCollectionFileAggregatorModel, UpdateTestCollectionFileAggregatorModel> _service;

        public CollectionFileAggregatorServiceTest(CollectionFileAggregatorServiceCollectionServiceFixture dbContextFixture, CollectionFileAggregatorServiceFileFixture fileFixture)
        {
            var collectionFileAggregatorSettings = new CollectionFileAggregatorSettings
            {
                ContainerName = fileFixture.Path,
                CollectionsFileFields = new Dictionary<string, string[]>
                {
                    {
                        TestData.CollectionName,
                        new string[]
                        {
                            TestData.FileFieldName
                        }
                    }
                }
            };
            _service = new CollectionFileAggregatorServiceMock(
                new CollectionServiceMock(
                    new CollectionRepositoryMock(dbContextFixture.DbContext, dbContextFixture.Mapper)
                ),
                fileFixture.FileService,
                dbContextFixture.Mapper,
                collectionFileAggregatorSettings
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
            var testCollectionFileAggregatorItemModel = await _service.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.Collection(
                testCollectionFileAggregatorItemModel.Items,
                item => Assert.NotNull(item.Images.Single().FieldName),
                item => Assert.NotNull(item.Images.Single().Value)
            );
        }

        [Fact]
        public async Task GetFilteredCollectionItems_ReturnsItemsWithFiles()
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
            var testCollectionFileAggregatorItemModel = await _service.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.Collection(
                testCollectionFileAggregatorItemModel.Items,
                item => Assert.NotNull(item.Images.Single().FieldName),
                item => Assert.NotNull(item.Images.Single().Value)
            );
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Arrange
            var createCollectionFileAggregatorModel = TestData.CreateCreateTestCollectionFileAggregatorModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.EntityIdToCreate
                    }
                }
            );

            // Act
            var id = await _service.CreateItem(createCollectionFileAggregatorModel);

            // Assert
            Assert.True(id == TestData.EntityIdToCreate);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 3
                        }
                    })
                }
            });

            // Act
            var testCollectionFileAggregatorItemModel = await _service.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
        {
            // Arrange
            await _service.UpdateItem(TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 4
                        }
                    })
                }
            }));

            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 4
                        }
                    })
                }
            });

            // Act
            var testCollectionFileAggregatorItemModel = await _service.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemRemoveCollectionItem_ReturnsRemovedItem()
        {
            // Arrange
            await _service.UpdateItem(TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 5
                        }
                    })
                }
            }));

            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>
                    {
                        {
                            "Id", 5
                        }
                    })
                }
            });

            // Act
            var testCollectionFileAggregatorItemModel = await _service.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().FieldName);
            Assert.NotNull(testCollectionFileAggregatorItemModel.Images.Single().Value);
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
