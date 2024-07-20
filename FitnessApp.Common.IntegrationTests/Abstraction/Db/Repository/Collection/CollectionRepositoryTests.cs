using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Repository.Collection;

[Collection("CollectionRepository collection")]
public class CollectionRepositoryTests : IClassFixture<CollectionRepositoryDbContextFixture>
{
    private readonly ICollectionRepository<
        TestCollectionModel,
        TestCollectionItemModel,
        CreateTestCollectionModel,
        UpdateTestCollectionModel> _repository;

    public CollectionRepositoryTests(CollectionRepositoryDbContextFixture fixture)
    {
        _repository = new CollectionRepositoryMock(fixture.DbContext, fixture.Mapper);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Act
        var item = await _repository.GetItemByUserId(TestData.EntityIdToGet);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToGet, item.UserId);
    }

    [Fact]
    public async Task GetCollectionByUserId_ReturnsCollection()
    {
        // Act
        var collection = await _repository.GetCollectionByUserId(TestData.EntityIdToGet, TestData.CollectionName);

        // Assert
        Assert.NotNull(collection);
        Assert.NotEmpty(collection);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
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
    public async Task UpdateItem_AddCollectionItem_ReturnsAddedItem()
    {
        // Arrange
        var addCollectionItemModel = TestData.CreateCollectionItemModel(Guid.NewGuid());

        // Act
        var updatedItem = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Add, addCollectionItemModel));
        var updatedCollection = await _repository.GetCollectionByUserId(TestData.EntityIdToUpdate, TestData.CollectionName);

        // Assert
        Assert.NotNull(updatedItem);
        Assert.Equal(addCollectionItemModel.Id, updatedItem.Id);
        Assert.Equal(addCollectionItemModel.TestProperty, updatedItem.TestProperty);
        Assert.Contains(updatedItem, updatedCollection);
    }

    [Fact]
    public async Task UpdateItem_UpdatesCollectionItem_ReturnsUpdatedItem()
    {
        // Arrange
        var collection = await _repository.GetCollectionByUserId(TestData.EntityIdToUpdate, TestData.CollectionName);
        var updateCollectionItemModel = collection.First();
        updateCollectionItemModel.TestProperty = "Updated";

        // Act
        var updatedItem = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Update, updateCollectionItemModel));
        var updatedCollection = await _repository.GetCollectionByUserId(TestData.EntityIdToUpdate, TestData.CollectionName);

        // Assert
        Assert.NotNull(updatedItem);
        Assert.Equal(updateCollectionItemModel.Id, updatedItem.Id);
        Assert.Equal(updateCollectionItemModel.TestProperty, updatedItem.TestProperty);
        Assert.Contains(updatedItem, updatedCollection);
    }

    [Fact]
    public async Task UpdateItem_DeletesCollectionItem_ReturnsDeletedItem()
    {
        // Arrange
        var collection = await _repository.GetCollectionByUserId(TestData.EntityIdToUpdate, TestData.CollectionName);
        var deleteCollectionItemModel = collection.First();

        // Act
        var updatedItem = await _repository.UpdateItem(CreateUpdateTestCollectionModel(UpdateCollectionAction.Remove, deleteCollectionItemModel));
        var updatedCollection = await _repository.GetCollectionByUserId(TestData.EntityIdToUpdate, TestData.CollectionName);

        // Assert
        Assert.NotNull(updatedItem);
        Assert.Equal(deleteCollectionItemModel.Id, updatedItem.Id);
        Assert.Equal(deleteCollectionItemModel.TestProperty, updatedItem.TestProperty);
        Assert.DoesNotContain(updatedItem, updatedCollection);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeleted()
    {
        // Act
        var deletedModel = await _repository.DeleteItem(TestData.EntityIdToDelete);

        // Assert
        Assert.Equal(TestData.EntityIdToDelete, deletedModel.UserId);
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
