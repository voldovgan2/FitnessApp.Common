using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Exceptions;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Collection;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.Collection;

public class CollectionServiceTest : TestBase
{
    private readonly Mock<
        ICollectionRepository<
            TestCollectionModel,
            TestCollectionItemModel,
            CreateTestCollectionModel,
            UpdateTestCollectionModel>
        > _repository;
    private readonly CollectionServiceMock _service;
    private readonly Dictionary<string, object> _defaultTestCollectionEntityParameters = new()
    {
        {
            "Id", TestData.Id
        },
        {
            "ItemsCount", 2
        }
    };

    public CollectionServiceTest() : base()
    {
        _repository = new Mock<
            ICollectionRepository<
                TestCollectionModel,
                TestCollectionItemModel,
                CreateTestCollectionModel,
                UpdateTestCollectionModel>
            >();
        _service = new CollectionServiceMock(_repository.Object);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetItemByUserId_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.GetItemByUserId(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Arrange
        var genericModelsMock = TestData.GetAll(TestData.CreateCollectionModel, new()
        {
            {
                "ItemsCount", 2
            }
        });
        _repository
           .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
           .ReturnsAsync(genericModelsMock.Single(i => i.UserId == TestData.Id));

        // Act
        var entity = await _service.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetCollectionByUserId_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetCollectionByUserId(userId, "collection"));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetCollectionByUserId_CollectionIsEmpty_ThrowsValidationError(string collection)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetCollectionByUserId("userId", collection));
        Assert.Equal("Field validation failed, field name: collectionName, message: collectionName can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetCollectionByUserId_ReturnsCollection()
    {
        // Arrange
        var collectionItems = new List<TestCollectionItemModel>
        {
            TestData.CreateCollectionItemModel(1),
            TestData.CreateCollectionItemModel(2)
        };
        _repository
           .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
           .ReturnsAsync(collectionItems);

        // Act
        var collection = await _service.GetCollectionByUserId(TestData.Id, TestData.CollectionName);

        // Assert
        Assert.All(collection, c => Assert.Contains(collectionItems, ci => ci.Id == c.Id));
        Assert.All(collection, c => Assert.Equal(c.TestProperty, collectionItems.Single(ci => ci.Id == c.Id).TestProperty));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredCollectionItems_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetFilteredCollectionItems(new GetTestFilteredCollectionItemsModel
        {
            UserId = userId,
            CollectionName = TestData.CollectionName,
            Page = 0,
            PageSize = 10
        }));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetFilteredCollectionItems_CollectionIsEmpty_ThrowsValidationError(string collection)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetFilteredCollectionItems(new GetTestFilteredCollectionItemsModel
        {
            UserId = TestData.Id,
            CollectionName = collection,
            Page = 0,
            PageSize = 10
        }));
        Assert.Equal("Field validation failed, field name: CollectionName, message: CollectionName can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetFilteredCollectionItems_PagedPageLessThen0_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetFilteredCollectionItems(new GetTestFilteredCollectionItemsModel
        {
            UserId = TestData.Id,
            CollectionName = TestData.CollectionName,
            Page = -1,
            PageSize = 10
        }));
        Assert.Equal("Field validation failed, field name: Page, message: Page should be within the range [0, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetFilteredCollectionItems_PagedPageSizeLessThen1_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetFilteredCollectionItems(new GetTestFilteredCollectionItemsModel
        {
            UserId = TestData.Id,
            CollectionName = TestData.CollectionName,
            Page = 0,
            PageSize = 0
        }));
        Assert.Equal("Field validation failed, field name: PageSize, message: PageSize should be within the range [1, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetFilteredCollectionItems_ReturnsMatchedItems()
    {
        // Arrange
        var collectionItems = new List<TestCollectionItemModel>
        {
            TestData.CreateCollectionItemModel(1),
            TestData.CreateCollectionItemModel(2),
            TestData.CreateCollectionItemModel(3),
            TestData.CreateCollectionItemModel(4)
        };
        var filteredBySearchItems = collectionItems.Take(2).Select(i => i.Id);

        var getModel = new GetTestFilteredCollectionItemsModel
        {
            UserId = TestData.Id,
            CollectionName = TestData.CollectionName,
            Page = 0,
            PageSize = 10
        };

        _repository
           .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
           .ReturnsAsync(collectionItems);

        // Act
        var items = await _service.GetFilteredCollectionItems(getModel);

        // Assert
        Assert.All(items.Items, m => filteredBySearchItems.Contains(m.Id));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.CreateItem(TestData.CreateCreateTestGenericModel(CreateGenericModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
        // Arrange
        var createModel = TestData.CreateCreateTestCollectionModel(new Dictionary<string, object>
        {
            {
                "Id", TestData.Id
            }
        });

        _repository
           .Setup(s => s.CreateItem(It.IsAny<CreateTestCollectionModel>()))
           .ReturnsAsync(createModel.UserId);

        // Act
        var id = await _service.CreateItem(createModel);

        // Assert
        Assert.Equal(TestData.Id, id);
    }

    [Fact]
    public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
    {
        // Arrange
        var addItemModel = TestData.CreateCollectionItemModel(1);

        _repository
           .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
           .ReturnsAsync(addItemModel);

        // Act
        var createdItemModel = await _service.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Add, addItemModel));

        // Assert
        Assert.Equal(addItemModel.Id, createdItemModel.Id);
        Assert.Equal(addItemModel.TestProperty, createdItemModel.TestProperty);
    }

    [Fact]
    public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
    {
        // Arrange
        var updateItemModel = TestData.CreateCollectionItemModel(1, "Updated");

        _repository
           .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
           .ReturnsAsync(updateItemModel);

        // Act
        var createdItemModel = await _service.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Update, TestData.CreateCollectionItemModel(1)));

        // Assert
        Assert.Equal(updateItemModel.Id, createdItemModel.Id);
        Assert.Equal(updateItemModel.TestProperty, createdItemModel.TestProperty);
    }

    [Fact]
    public async Task UpdateItemDeleteCollectionItem_ReturnsDeletedItem()
    {
        // Arrange
        var updateItemModel = TestData.CreateCollectionItemModel(1);

        _repository
           .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
           .ReturnsAsync(updateItemModel);

        // Act
        var deletedItemModel = await _service.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Remove, TestData.CreateCollectionItemModel(1)));

        // Assert
        Assert.Equal(updateItemModel.Id, deletedItemModel.Id);
        Assert.Equal(updateItemModel.TestProperty, deletedItemModel.TestProperty);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeleted()
    {
        // Arrange
        var deletedModel = TestData.CreateCollectionModel(_defaultTestCollectionEntityParameters);

        _repository
           .Setup(s => s.DeleteItem(It.IsAny<string>()))
           .ReturnsAsync(deletedModel);

        // Act
        var deletedItemModel = await _service.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(deletedModel.UserId, deletedItemModel.UserId);
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
