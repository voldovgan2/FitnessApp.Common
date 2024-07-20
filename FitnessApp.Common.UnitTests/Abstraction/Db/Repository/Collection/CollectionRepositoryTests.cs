using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Db.Repository.Collection;

public class CollectionRepositoryTests : TestBase
{
    private readonly Mock<IDbContext<TestCollectionEntity>> _dbContextMock;
    private readonly CollectionRepositoryMock _repository;
    private readonly Dictionary<string, object> _defaultTestCollectionEntityParameters = new()
    {
        {
            "Id", TestData.Id
        },
        {
            "ItemsCount", 2
        }
    };

    public CollectionRepositoryTests() : base()
    {
        _dbContextMock = new Mock<IDbContext<TestCollectionEntity>>();
        _repository = new CollectionRepositoryMock(_dbContextMock.Object, _mapper);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsAllItems()
    {
        // Arrange
        var collectionEntitiesMock = GetCollectionEntitiesMock();
        _dbContextMock
            .Setup(s => s.GetItemById(It.IsAny<string>()))
            .ReturnsAsync(collectionEntitiesMock.Single(i => i.UserId == TestData.Id));

        // Act
        var collectionModel = await _repository.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, collectionModel.UserId);
        Assert.Equal(TestData.CollectionName, collectionModel.Collection.Keys.Single());
    }

    [Fact]
    public async Task GetCollectionByUserId_ReturnsCollection()
    {
        // Arrange
        var allEntities = TestData.GetAll(
            TestData.CreateCollectionEntity,
            new Dictionary<string, object>
            {
                {
                    "ItemsCount", 2
                }
            }
        );
        _dbContextMock
            .Setup(s => s.GetItemById(It.IsAny<string>()))
            .ReturnsAsync(allEntities.Single(i => i.UserId == TestData.Id));

        var expectedCollection = _mapper.Map<IEnumerable<TestCollectionItemModel>>(allEntities.Single(e => e.UserId == TestData.Id).Collection[TestData.CollectionName]);

        // Act
        var returnedCollection = (await _repository.GetCollectionByUserId(TestData.Id, TestData.CollectionName)).ToList();

        // Assert
        Assert.All(returnedCollection, ri => Assert.Equal(ri.TestProperty, expectedCollection.Single(ei => ei.Id == ri.Id).TestProperty));
    }

    [Fact]
    public async Task CreateItem_ReturnsAllItems()
    {
        // Arrange
        var entity = TestData.CreateCollectionEntity(_defaultTestCollectionEntityParameters);
        _dbContextMock
             .Setup(s => s.CreateItem(It.IsAny<TestCollectionEntity>()))
             .ReturnsAsync(entity);

        // Act
        var createdId = await _repository.CreateItem(TestData.CreateCreateTestCollectionModel(new Dictionary<string, object>
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
        var collectionEntitiesMock = GetCollectionEntitiesMock();
        _dbContextMock
            .Setup(s => s.GetItemById(It.IsAny<string>()))
            .ReturnsAsync(collectionEntitiesMock.Single(i => i.UserId == TestData.Id));

        var entityUpdate = TestData.CreateCollectionEntity(_defaultTestCollectionEntityParameters);
        _dbContextMock
            .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
            .ReturnsAsync(entityUpdate);

        var addItemModel = TestData.CreateCollectionItemModel(3);

        // Act
        var createdItemModel = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Add, addItemModel));

        // Assert
        Assert.Equal(addItemModel.Id, createdItemModel.Id);
        Assert.Equal(addItemModel.TestProperty, createdItemModel.TestProperty);
    }

    [Fact]
    public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatededItem()
    {
        // Arrange
        var collectionEntitiesMock = GetCollectionEntitiesMock();
        _dbContextMock
            .Setup(s => s.GetItemById(It.IsAny<string>()))
            .ReturnsAsync(collectionEntitiesMock.Single(i => i.UserId == TestData.Id));

        var entityUpdate = TestData.CreateCollectionEntity(_defaultTestCollectionEntityParameters);
        _dbContextMock
            .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
            .ReturnsAsync(entityUpdate);

        var updateItemModel = TestData.CreateCollectionItemModel(1, "Updated");

        // Act
        var updatedItemModel = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Update, updateItemModel));

        // Assert
        Assert.Equal(updateItemModel.Id, updatedItemModel.Id);
        Assert.Equal(updateItemModel.TestProperty, updatedItemModel.TestProperty);
    }

    [Fact]
    public async Task UpdateItemDeleteCollectionItem_ReturnsDeletedItem()
    {
        // Arrange
        var collectionEntitiesMock = GetCollectionEntitiesMock();
        _dbContextMock
            .Setup(s => s.GetItemById(It.IsAny<string>()))
            .ReturnsAsync(collectionEntitiesMock.Single(i => i.UserId == TestData.Id));

        var entityUpdate = TestData.CreateCollectionEntity(_defaultTestCollectionEntityParameters);
        _dbContextMock
            .Setup(s => s.UpdateItem(It.IsAny<TestCollectionEntity>()))
            .ReturnsAsync(entityUpdate);

        var deleteItemModel = TestData.CreateCollectionItemModel(1);

        // Act
        var deletedItemModel = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Remove, deleteItemModel));

        // Assert
        Assert.Equal(deleteItemModel.Id, deletedItemModel.Id);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItem()
    {
        // Arrange
        var entity = TestData.CreateCollectionEntity(_defaultTestCollectionEntityParameters);

        _dbContextMock
           .Setup(s => s.DeleteItem(It.IsAny<string>()))
           .ReturnsAsync(entity);

        // Act
        var deletedModel = await _repository.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(entity.UserId, deletedModel.UserId);
    }

    private UpdateTestCollectionModel CreateUpdateTestCollectionModel(UpdateCollectionAction action, TestCollectionItemModel model)
    {
        return TestData.CreateUpdateTestCollectionModel(new Dictionary<string, object>
        {
            {
                "Id", TestData.Id
            },
            {
                "Action", action
            },
            {
                "CollectionName", TestData.CollectionName
            },
            {
                "Model", model
            },
        });
    }
}
