using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.Collection
{
    public class CollectionServiceTest : TestBase
    {
        [Fact]
        public async Task GetItemByUserId_ReturnsSingleItem()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var queryableGenericModelsMock = GetQueryableMock(
                TestData.GetAll(
                    TestData.CreateCollectionModel,
                    new Dictionary<string, object>
                    {
                        {
                            "ItemsCount", 2
                        }
                    }
                )
            );
            repositoryMock
               .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
               .ReturnsAsync(queryableGenericModelsMock.Object.Single(i => i.UserId == TestData.Id));

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var entity = await service.GetItemByUserId(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, entity.UserId);
        }

        [Fact]
        public async Task GetCollectionByUserId_ReturnsCollection()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var collectionItems = new List<TestCollectionItemModel>
            {
                TestData.CreateCollectionItemModel(1),
                TestData.CreateCollectionItemModel(2)
            };
            repositoryMock
               .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(collectionItems);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var collection = await service.GetCollectionByUserId(TestData.Id, TestData.CollectionName);

            // Assert
            Assert.All(collection, c => Assert.Contains(collectionItems, ci => ci.Id == c.Id));
            Assert.All(collection, c => Assert.Equal(c.TestProperty, collectionItems.Single(ci => ci.Id == c.Id).TestProperty));
        }

        [Fact]
        public async Task GetFilteredCollectionItems_ReturnsMatchedItems()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var collectionItems = new List<TestCollectionItemModel>
            {
                TestData.CreateCollectionItemModel(1),
                TestData.CreateCollectionItemModel(2),
                TestData.CreateCollectionItemModel(3),
                TestData.CreateCollectionItemModel(4)
            };
            var filteredBySearchItems = collectionItems.Take(2).Select(i => i.Id);
            searchMock
                .Setup(s => s.Search(It.IsAny<string>()))
                .ReturnsAsync(filteredBySearchItems);
            var testProperty = "TestProperty1";

            var getModel = new GetTestFilteredCollectionItemsModel
            {
                UserId = TestData.Id,
                CollectionName = TestData.CollectionName,
                Predicate = itemModel => itemModel.TestProperty == testProperty,
                Page = 0,
                PageSize = 10
            };

            repositoryMock
               .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync(collectionItems);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var items = await service.GetFilteredCollectionItems("", getModel);

            // Assert
            Assert.All(items.Items, m => filteredBySearchItems.Contains(m.Id));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var createModel = TestData.CreateCreateTestCollectionModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                }
            });

            repositoryMock
               .Setup(s => s.CreateItem(It.IsAny<CreateTestCollectionModel>()))
               .ReturnsAsync(createModel.UserId);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var id = await service.CreateItem(createModel);

            // Assert
            Assert.Equal(TestData.Id, id);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var addItemModel = TestData.CreateCollectionItemModel(1);

            repositoryMock
               .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
               .ReturnsAsync(addItemModel);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var createdItemModel = await service.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Action", UpdateCollectionAction.Add
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Model", addItemModel
                },
            }));

            // Assert
            Assert.Equal(addItemModel.Id, createdItemModel.Id);
            Assert.Equal(addItemModel.TestProperty, createdItemModel.TestProperty);
        }

        [Fact]
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var updateItemModel = TestData.CreateCollectionItemModel(1, "Updated");

            repositoryMock
               .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
               .ReturnsAsync(updateItemModel);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var createdItemModel = await service.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Action", UpdateCollectionAction.Update
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Model", TestData.CreateCollectionItemModel(1)
                },
            }));

            // Assert
            Assert.Equal(updateItemModel.Id, createdItemModel.Id);
            Assert.Equal(updateItemModel.TestProperty, createdItemModel.TestProperty);
        }

        [Fact]
        public async Task UpdateItemDeleteCollectionItem_ReturnsDeletedItem()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();
            var updateItemModel = TestData.CreateCollectionItemModel(1);

            repositoryMock
               .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
               .ReturnsAsync(updateItemModel);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var deletedItemModel = await service.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Action", UpdateCollectionAction.Remove
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Model", TestData.CreateCollectionItemModel(1)
                },
            }));

            // Assert
            Assert.Equal(updateItemModel.Id, deletedItemModel.Id);
            Assert.Equal(updateItemModel.TestProperty, deletedItemModel.TestProperty);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeleted()
        {
            // Arrange
            var repositoryMock = new Mock<ICollectionRepository<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            var searchMock = new Mock<ISearchService>();

            var deletedModel = TestData.CreateCollectionModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    },
                    {
                        "ItemsCount", 2
                    }
                }
            );

            repositoryMock
               .Setup(s => s.DeleteItem(It.IsAny<string>()))
               .ReturnsAsync(deletedModel);

            var service = new CollectionServiceMock(repositoryMock.Object, searchMock.Object);

            // Act
            var deletedItemModel = await service.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(deletedModel.UserId, deletedItemModel.UserId);
        }
    }
}
