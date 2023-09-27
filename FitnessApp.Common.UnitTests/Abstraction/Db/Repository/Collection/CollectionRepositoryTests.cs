using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Db.Repository.Collection
{
    public class CollectionRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetItemByUserId_ReturnsAllItems()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            var queryableCollectionEntitiesMock = GetQueryableCollectionEntitiesMock();
            dbContextMock
                .Setup(s => s.GetItemById(It.IsAny<string>()))
                .ReturnsAsync(queryableCollectionEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var collectionModel = await repository.GetItemByUserId(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, collectionModel.UserId);
            Assert.Equal(TestData.CollectionName, collectionModel.Collection.Keys.Single());
        }

        [Fact]
        public async Task GetCollectionByUserId_ReturnsCollection()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            var allEntities = TestData.GetAll(
                TestData.CreateCollectionEntity,
                new Dictionary<string, object>
                {
                    {
                        "ItemsCount", 2
                    }
                }
            );
            var queryableCollectionEntitiesMock = GetQueryableCollectionEntitiesMock(allEntities);
            dbContextMock
                .Setup(s => s.GetItemById(It.IsAny<string>()))
                .ReturnsAsync(queryableCollectionEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            var expectedCollection = _mapper.Map<IEnumerable<TestCollectionItemModel>>(allEntities.Single(e => e.UserId == TestData.Id).Collection[TestData.CollectionName]);

            // Act
            var returnedCollection = (await repository.GetCollectionByUserId(TestData.Id, TestData.CollectionName)).ToList();

            // Assert
            Assert.All(returnedCollection, ri => Assert.Equal(ri.TestProperty, expectedCollection.Single(ei => ei.Id == ri.Id).TestProperty));
        }

        [Fact]
        public async Task CreateItem_ReturnsAllItems()
        {
            // Arrange
            var entity = TestData.CreateCollectionEntity(
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

            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            dbContextMock
                 .Setup(s => s.CreateItem(It.IsAny<TestCollectionEntity>()))
                 .ReturnsAsync(entity);

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var createdId = await repository.CreateItem(TestData.CreateCreateTestCollectionModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                }
            }));

            // Assert
            Assert.Equal(TestData.Id, createdId);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            var queryableCollectionEntitiesMock = GetQueryableCollectionEntitiesMock();
            dbContextMock
                .Setup(s => s.GetItemById(It.IsAny<string>()))
                .ReturnsAsync(queryableCollectionEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var entityUpdate = TestData.CreateCollectionEntity(
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
            dbContextMock
                .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
                .ReturnsAsync(entityUpdate);

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            var addItemModel = TestData.CreateCollectionItemModel(3);

            // Act
            var createdItemModel = await repository.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
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
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatededItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            var queryableCollectionEntitiesMock = GetQueryableCollectionEntitiesMock();
            dbContextMock
                .Setup(s => s.GetItemById(It.IsAny<string>()))
                .ReturnsAsync(queryableCollectionEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var entityUpdate = TestData.CreateCollectionEntity(
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
            dbContextMock
                .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
                .ReturnsAsync(entityUpdate);

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            var updateItemModel = TestData.CreateCollectionItemModel(1, "Updated");

            // Act
            var updatedItemModel = await repository.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
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
                    "Model", updateItemModel
                },
            }));

            // Assert
            Assert.Equal(updateItemModel.Id, updatedItemModel.Id);
            Assert.Equal(updateItemModel.TestProperty, updatedItemModel.TestProperty);
        }

        [Fact]
        public async Task UpdateItemDeleteCollectionItem_ReturnsDeletedItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            var queryableCollectionEntitiesMock = GetQueryableCollectionEntitiesMock();
            dbContextMock
                .Setup(s => s.GetItemById(It.IsAny<string>()))
                .ReturnsAsync(queryableCollectionEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var entityUpdate = TestData.CreateCollectionEntity(
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
            dbContextMock
                .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
                .ReturnsAsync(entityUpdate);

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            var deleteItemModel = TestData.CreateCollectionItemModel(1);

            // Act
            var deletedItemModel = await repository.UpdateItem(TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
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
                    "Model", deleteItemModel
                },
            }));

            // Assert
            Assert.Equal(deleteItemModel.Id, deletedItemModel.Id);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItem()
        {
            // Arrange
            var entity = TestData.CreateCollectionEntity(
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

            var dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
            dbContextMock
               .Setup(s => s.DeleteItem(It.IsAny<string>()))
               .ReturnsAsync(entity);

            var repository = new CollectionRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var deletedModel = await repository.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(entity.UserId, deletedModel.UserId);
        }
    }
}
