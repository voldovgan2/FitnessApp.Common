using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.IntegrationTests.Abstraction.Db.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Db.DbContext;

[Collection("DbContext collection")]
public class DbContextTest(GenericDbContextFixture fixture) : IClassFixture<GenericDbContextFixture>
{
    [Fact]
    public async Task GetItemById_ReturnsSingleItem()
    {
        // Act
        var item = await fixture.DbContext.GetItemById(TestData.EntityIdToGet);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToGet, item.UserId);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
        // Act
        var item = await fixture.DbContext.CreateItem(TestData.CreateGenericEntity(new Dictionary<string, object>
        {
            {
                "Id", TestData.EntityIdToCreate
            }
        }));

        // Assert
        Assert.NotNull(item);
        Assert.Contains(TestData.EntityIdToCreate, item.UserId);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdated()
    {
        // Arrange
        var existingItem = await fixture.DbContext.GetItemById(TestData.EntityIdToUpdate);
        existingItem.TestProperty1 = "Updated";

        // Act
        var updatedItem = await fixture.DbContext.UpdateItem(existingItem);

        // Assert
        Assert.NotNull(updatedItem);
        Assert.Equal(TestData.EntityIdToUpdate, updatedItem.UserId);
        Assert.Equal(existingItem.TestProperty1, updatedItem.TestProperty1);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeleted()
    {
        // Act
        var item = await fixture.DbContext.DeleteItem(TestData.EntityIdToDelete);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToDelete, item.UserId);
    }
}
