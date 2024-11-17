using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.UnitTests.Abstraction;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstractions.Db.Repository.Generic;

public abstract class GenericRepositoryTests : TestBase
{
    private readonly Mock<IDbContext<TestGenericEntity>> _dbContextMock;
    private readonly GenericRepositoryMock _repository;
    private readonly Dictionary<string, object> _defaultGenericModelParameters = new()
    {
        {
            "Id", TestData.Id
        }
    };

    protected GenericRepositoryTests()
    {
        _dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
        _repository = new GenericRepositoryMock(_dbContextMock.Object, _mapper);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Arrange
        var genericEntitiesMock = GetGenericEntitiesMock();
        _dbContextMock
           .Setup(s => s.GetItemById(It.IsAny<string>()))
           .ReturnsAsync(genericEntitiesMock.Single(i => i.UserId == TestData.Id));

        // Act
        var entity = await _repository.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task GetItemByIds_ReturnsMathcedItems()
    {
        // Arrange
        var genericEntitiesMock = GetGenericEntitiesMock();
        _dbContextMock
           .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
           .ReturnsAsync(genericEntitiesMock.Where(i => i.UserId == TestData.Id));

        // Act
        var entity = (await _repository.GetItemsByIds([TestData.Id])).Single();

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task GetItems_ReturnsMathcedItems()
    {
        // Arrange
        var genericEntitiesMock = GetGenericEntitiesMock();
        _dbContextMock
           .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
           .ReturnsAsync(genericEntitiesMock.Where(i => i.UserId == TestData.Id));

        // Act
        var entity = (await _repository.GetItems(new GetTestPagedByIdsDataModel
        {
            Ids = new List<string> { TestData.Id },
            Page = 0,
            PageSize = 10,
        })).Items.Single();

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Arrange
        var createdEntity = TestData.CreateGenericEntity(_defaultGenericModelParameters);
        _dbContextMock
            .Setup(s => s.CreateItem(It.IsAny<TestGenericEntity>()))
            .ReturnsAsync(createdEntity);

        var createModel = TestData.CreateCreateTestGenericModel(_defaultGenericModelParameters);

        // Act
        var entity = await _repository.CreateItem(createModel);

        // Assert
        Assert.Equal(createModel.UserId, entity.UserId);
        Assert.Equal(createModel.TestProperty1, entity.TestProperty1);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Arrange
        var genericEntitiesMock = GetGenericEntitiesMock();
        _dbContextMock
           .Setup(s => s.GetItemById(It.IsAny<string>()))
           .ReturnsAsync(genericEntitiesMock.Single(i => i.UserId == TestData.Id));

        var entityUpdate = TestData.CreateGenericEntity(_defaultGenericModelParameters);
        entityUpdate.TestProperty1 = "Updated";

        _dbContextMock
            .Setup(s => s.UpdateItem(It.IsAny<TestGenericEntity>()))
            .ReturnsAsync(entityUpdate);

        var updateModel = TestData.CreateUpdateTestGenericModel(_defaultGenericModelParameters);
        updateModel.TestProperty1 = "Updated";

        // Act
        var entity = await _repository.UpdateItem(updateModel);

        // Assert
        Assert.Equal(updateModel.TestProperty1, entity.TestProperty1);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Arrange
        var entity = TestData.CreateGenericEntity(_defaultGenericModelParameters);
        _dbContextMock
            .Setup(s => s.DeleteItem(It.IsAny<string>()))
            .ReturnsAsync(entity);

        // Act
        var deletedId = await _repository.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, deletedId);
    }
}