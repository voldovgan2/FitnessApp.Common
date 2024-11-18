using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.Repository.Generic;

[Collection("GenericRepository collection")]
public abstract class GenericRepositoryTest : IClassFixture<GenericRepositoryDbContextFixture>
{
    private readonly GenericRepositoryMock _repository;

    protected GenericRepositoryTest(GenericRepositoryDbContextFixture fixture)
    {
        _repository = new GenericRepositoryMock(fixture.DbContext, fixture.Mapper);
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
    public async Task CreateItem_ReturnsCreated()
    {
        // Act
        var item = await _repository.CreateItem(TestData.CreateCreateTestGenericModel(new Dictionary<string, object>
        {
            {
                "Id", TestData.EntityIdToCreate
            }
        }));

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToCreate, item.UserId);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdated()
    {
        // Arrange
        var existingItem = await _repository.GetItemByUserId(TestData.EntityIdToUpdate);
        existingItem.TestProperty1 = "Updated";

        // Act
        var updatedItem = await _repository.UpdateItem(new UpdateTestGenericModel
        {
            UserId = existingItem.UserId,
            TestProperty1 = existingItem.TestProperty1
        });

        // Assert
        Assert.NotNull(updatedItem);
        Assert.Equal(TestData.EntityIdToUpdate, updatedItem.UserId);
        Assert.Equal(existingItem.TestProperty1, updatedItem.TestProperty1);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeleted()
    {
        // Act
        var itemId = await _repository.DeleteItem(TestData.EntityIdToDelete);

        // Assert
        Assert.Equal(TestData.EntityIdToDelete, itemId);
    }
}
