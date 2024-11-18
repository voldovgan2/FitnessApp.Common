using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Abstractions.Services;
using FitnessApp.Common.Exceptions;
using Xunit;

namespace FitnessApp.Common.UnitTests;

public abstract class GenericServiceTestBase<
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel> :
    TestBase<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    protected readonly GenericService<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel> Service;

    protected GenericServiceTestBase(
        GenericService<
            TGenericModel,
            TCreateGenericModel,
            TUpdateGenericModel> service)
    {
        Service = service;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetItemByUserId_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Service.GetItemByUserId(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Act
        var entity = await Service.GetItemByUserId(Id);

        // Assert
        AssertExtractedItem(entity);
    }

    [Fact]
    public async Task GetItemByIds_IdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Service.GetItemByUserIds([]));
        Assert.Equal("Field validation failed, field name: ids, message: ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsItems()
    {
        // Act
        var models = await Service.GetItemByUserIds(Ids);

        // Assert
        AssertCollection(models);
    }

    [Fact]
    public async Task GetItemByIds_PagedIdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Service.GetItems(CreateGetPagedByIdsDataModel([], 1, 1)));
        Assert.Equal("Field validation failed, field name: Ids, message: Ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageLessThen0_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => Service.GetItems(CreateGetPagedByIdsDataModel(Ids, -1, 1)));
        Assert.Equal("Field validation failed, field name: Page, message: Page should be within the range [0, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageSizeLessThen1_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => Service.GetItems(CreateGetPagedByIdsDataModel(Ids, 0, 0)));
        Assert.Equal("Field validation failed, field name: PageSize, message: PageSize should be within the range [1, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsPagedItems()
    {
        // Act
        var models = await Service.GetItems(CreateGetPagedByIdsDataModel(Ids, 1, 1));

        // Assert
        AssertCollection(models.Items);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => Service.CreateItem(CreateCreateModel(CreateGenericModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Act
        var entity = await Service.CreateItem(CreateCreateModel(CreateGenericModelParameters(Id)));

        // Assert
        AssertCreatedItem(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => Service.UpdateItem(CreateUpdateModel(CreateGenericModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Act
        var entity = await Service.UpdateItem(CreateUpdateModel(CreateGenericModelParameters(Id)));

        // Assert
        AssertUpdatedItem(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteItem_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => Service.DeleteItem(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Act
        var deletedId = await Service.DeleteItem(Id);

        // Assert
        Assert.Equal(Id, deletedId);
    }

    protected override void AssertExtractedItem(TGenericModel model)
    {
        Assert.Equal(Id, model.UserId);
    }

    protected override void AssertCollection(TGenericModel[] models)
    {
        Assert.All(models, m => Assert.Contains(m.UserId, Ids));
        base.AssertCollection(models);
    }
}