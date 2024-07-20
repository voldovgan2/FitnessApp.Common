using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.Generic;

public class GenericServiceTest : TestBase
{
    private readonly Mock<IGenericRepository<
        TestGenericEntity,
        TestGenericModel,
        CreateTestGenericModel,
        UpdateTestGenericModel>> _repositoryMock;
    private readonly GenericServiceMock _serviceMock;
    private readonly Dictionary<string, object> _defaultGenericModelParameters = new()
    {
        {
            "Id", TestData.Id
        }
    };

    public GenericServiceTest() : base()
    {
        _repositoryMock = new Mock<
            IGenericRepository<
                TestGenericEntity,
                TestGenericModel,
                CreateTestGenericModel,
                UpdateTestGenericModel>>();
        _serviceMock = new GenericServiceMock(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Arrange
        var allDataMock = TestData.GetAll(TestData.CreateGenericModel, new Dictionary<string, object>());
        _repositoryMock
           .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
           .ReturnsAsync(allDataMock.Single(i => i.UserId == TestData.Id));

        // Act
        var entity = await _serviceMock.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task GetItems_ReturnsMatchedBySearchCriteriaItems()
    {
        // Arrange
        var allDataMock = CreateDefaultMockedData();
        _repositoryMock
           .Setup(s => s.GetAllItems(It.IsAny<Expression<Func<TestGenericEntity, bool>>>()))
           .ReturnsAsync(allDataMock);
        var filteredBySearchItems = allDataMock.Take(2).Select(i => i.UserId);

        var testProperty = "TestProperty1";

        // Act
        var models = await _serviceMock.GetItems("", e => e.TestProperty1 == testProperty);

        // Assert
        Assert.All(models, m => filteredBySearchItems.Contains(m.UserId));
    }

    [Fact]
    public async Task GetItems_ReturnsMatchedByIdsItems()
    {
        // Arrange
        var genericEntitiesMock = CreateDefaultMockedData();
        _repositoryMock
           .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
           .ReturnsAsync(genericEntitiesMock.Where(e => TestData.Ids.Contains(e.UserId)));

        // Act
        var models = await _serviceMock.GetItems(TestData.Ids);

        // Assert
        Assert.All(models, m => Assert.Contains(m.UserId, TestData.Ids));
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Arrange
        var model = TestData.CreateGenericModel(_defaultGenericModelParameters);
        _repositoryMock
            .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
            .ReturnsAsync(model);

        // Act
        var entity = await _serviceMock.CreateItem(TestData.CreateCreateTestGenericModel(_defaultGenericModelParameters));

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Arrange
        var updateModel = TestData.CreateUpdateTestGenericModel(_defaultGenericModelParameters);

        var model = TestData.CreateGenericModel(_defaultGenericModelParameters);
        model.TestProperty1 = updateModel.TestProperty1;

        _repositoryMock
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
            .ReturnsAsync(model);

        // Act
        var entity = await _serviceMock.UpdateItem(updateModel);

        // Assert
        Assert.Equal(model.TestProperty1, entity.TestProperty1);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Arrange
        _repositoryMock
            .Setup(s => s.DeleteItem(It.IsAny<string>()))
            .ReturnsAsync(TestData.Id);

        // Act
        var deletedId = await _serviceMock.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, deletedId);
    }

    private IEnumerable<TestGenericEntity> CreateDefaultMockedData()
    {
        return TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>());
    }
}