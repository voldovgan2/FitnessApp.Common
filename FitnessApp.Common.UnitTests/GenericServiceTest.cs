using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Exceptions;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests;

public abstract class GenericServiceTest : TestBase
{
    private readonly Mock<IGenericRepository<
        TestGenericModel,
        CreateTestGenericModel,
        UpdateTestGenericModel>> _repository;
    private readonly GenericServiceMock _service;

    protected GenericServiceTest()
    {
        _repository = new Mock<
            IGenericRepository<
                TestGenericModel,
                CreateTestGenericModel,
                UpdateTestGenericModel>>();
        _service = new GenericServiceMock(_repository.Object);
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
        var allDataMock = TestData.GetAll(TestData.CreateGenericModel, new Dictionary<string, object>());
        _repository
           .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
           .ReturnsAsync(allDataMock.Single(i => i.UserId == TestData.Id));

        // Act
        var entity = await _service.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Fact]
    public async Task GetItemByIds_IdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.GetItemByUserIds([]));
        Assert.Equal("Field validation failed, field name: ids, message: ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsItems()
    {
        // Arrange
        var genericEntitiesMock = CreateDefaultMockedData();
        _repository
           .Setup(s => s.GetItemByUserIds(It.IsAny<string[]>()))
           .ReturnsAsync([..genericEntitiesMock.Where(e => TestData.Ids.Contains(e.UserId))]);

        // Act
        var models = await _service.GetItemByUserIds(TestData.Ids);

        // Assert
        Assert.All(models, m => Assert.Contains(m.UserId, TestData.Ids));
    }

    [Fact]
    public async Task GetItemByIds_PagedIdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.GetItems(new GetTestPagedByIdsDataModel
        {
            UserIds = [],
            Page = 1,
            PageSize = 1,
        }));
        Assert.Equal("Field validation failed, field name: Ids, message: Ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageLessThen0_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetItems(new GetTestPagedByIdsDataModel
        {
            UserIds = TestData.Ids,
            Page = -1,
            PageSize = 1,
        }));
        Assert.Equal("Field validation failed, field name: Page, message: Page should be within the range [0, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageSizeLessThen1_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.GetItems(new GetTestPagedByIdsDataModel
        {
            UserIds = TestData.Ids,
            Page = 0,
            PageSize = 0,
        }));
        Assert.Equal("Field validation failed, field name: PageSize, message: PageSize should be within the range [1, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsPagedItems()
    {
        // Arrange
        var model = new GetTestPagedByIdsDataModel
        {
            UserIds = TestData.Ids,
            Page = 1,
            PageSize = 1,
        };

        var genericEntitiesMock = CreateDefaultMockedData();
        _repository
           .Setup(s => s.GetItems(It.IsAny<GetPagedByIdsDataModel>()))
           .ReturnsAsync(genericEntitiesMock.Where(e => TestData.Ids.Contains(e.UserId)).ToArray().ToPaged(model));

        // Act
        var models = await _service.GetItems(model);

        // Assert
        Assert.All(models.Items, m => Assert.Contains(m.UserId, TestData.Ids));
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
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Arrange
        var genericModelParameters = CreateGenericModelParameters(TestData.Id);
        var model = TestData.CreateGenericModel(genericModelParameters);
        _repository
            .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
            .ReturnsAsync(model);

        // Act
        var entity = await _service.CreateItem(TestData.CreateCreateTestGenericModel(genericModelParameters));

        // Assert
        Assert.Equal(TestData.Id, entity.UserId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _service.UpdateItem(TestData.CreateUpdateTestGenericModel(CreateGenericModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Arrange
        var genericModelParameters = CreateGenericModelParameters(TestData.Id);
        var updateModel = TestData.CreateUpdateTestGenericModel(genericModelParameters);

        var model = TestData.CreateGenericModel(genericModelParameters);
        model.TestProperty1 = updateModel.TestProperty1;

        _repository
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
            .ReturnsAsync(model);

        // Act
        var entity = await _service.UpdateItem(updateModel);

        // Assert
        Assert.Equal(model.TestProperty1, entity.TestProperty1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteItem_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.DeleteItem(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Arrange
        _repository
            .Setup(s => s.DeleteItem(It.IsAny<string>()))
            .ReturnsAsync(TestData.Id);

        // Act
        var deletedId = await _service.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, deletedId);
    }

    private static Dictionary<string, object> CreateGenericModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            }
        };
    }

    private static TestGenericModel[] CreateDefaultMockedData()
    {
        return TestData.GetAll(TestData.CreateGenericModel, []);
    }
}