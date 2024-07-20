using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.Generic;

[Collection("GenericService collection")]
public class GenericServiceTest : IClassFixture<GenericServiceFixture>
{
    private readonly IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel> _service;

    public GenericServiceTest(GenericServiceFixture fixture)
    {
        _service = new GenericServiceMock(new GenericRepositoryMock(fixture.DbContext, fixture.Mapper), fixture.Mapper);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Act
        var item = await _service.GetItemByUserId(TestData.EntityIdToGet);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToGet, item.UserId);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
        // Act
        var item = await _service.CreateItem(TestData.CreateCreateTestGenericModel(
            new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToCreate
                }
            }
        ));

        // Assert
        Assert.NotNull(item);
        Assert.Equal(TestData.EntityIdToCreate, item.UserId);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdated()
    {
        // Arrange
        var existingItem = await _service.GetItemByUserId(TestData.EntityIdToUpdate);
        existingItem.TestProperty1 = "Updated";

        // Act
        var updatedItem = await _service.UpdateItem(new UpdateTestGenericModel
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
        var itemId = await _service.DeleteItem(TestData.EntityIdToDelete);

        // Assert
        Assert.Equal(TestData.EntityIdToDelete, itemId);
    }
}
