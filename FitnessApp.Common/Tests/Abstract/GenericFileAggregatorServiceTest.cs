using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Abstractions.Services;
using FitnessApp.Common.Exceptions;
using Xunit;

namespace FitnessApp.Common.Tests.Abstract;

public abstract class GenericFileAggregatorServiceTest<
    TGenericFileAggregatorModel,
    TGenericModel,
    TCreateGenericFileAggregatorModel,
    TCreateGenericModel,
    TUpdateGenericFileAggregatorModel,
    TUpdateGenericModel> :
    TestBase<
        TGenericFileAggregatorModel,
        TCreateGenericFileAggregatorModel,
        TUpdateGenericFileAggregatorModel>
    where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    private readonly GenericFileAggregatorService<
        TGenericFileAggregatorModel,
        TGenericModel,
        TCreateGenericFileAggregatorModel,
        TCreateGenericModel,
        TUpdateGenericFileAggregatorModel,
        TUpdateGenericModel> _genericFileAggregatorService;

    protected GenericFileAggregatorServiceTest(
        GenericFileAggregatorService<
            TGenericFileAggregatorModel,
            TGenericModel,
            TCreateGenericFileAggregatorModel,
            TCreateGenericModel,
            TUpdateGenericFileAggregatorModel,
            TUpdateGenericModel> genericFileAggregatorService)
    {
        _genericFileAggregatorService = genericFileAggregatorService;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GetItemByUserId_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.GetItemByUserId(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Act
        var entity = await _genericFileAggregatorService.GetItemByUserId(Id);

        // Assert
        AssertExtractedItem(entity);
    }

    [Fact]
    public async Task GetItemByIds_IdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.GetItemsByUserIds([]));
        Assert.Equal("Field validation failed, field name: ids, message: ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsItems()
    {
        // Act
        var entities = await _genericFileAggregatorService.GetItemsByUserIds(Ids);

        // Assert
        AssertCollection(entities);
    }

    [Fact]
    public async Task GetItemByIds_PagedIdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.GetItems(CreateGetPagedByIdsDataModel([], 1, 1)));
        Assert.Equal("Field validation failed, field name: Ids, message: Ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageLessThen0_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.GetItems(CreateGetPagedByIdsDataModel(Ids, -1, 1)));
        Assert.Equal("Field validation failed, field name: Page, message: Page should be within the range [0, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageSizeLessThen1_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.GetItems(CreateGetPagedByIdsDataModel(Ids, 0, 0)));
        Assert.Equal("Field validation failed, field name: PageSize, message: PageSize should be within the range [1, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsPagedItems()
    {
        // Act
        var entities = await _genericFileAggregatorService.GetItems(CreateGetPagedByIdsDataModel(Ids, 1, 1));

        // Assert
        AssertCollection(entities.Items);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.CreateItem(CreateCreateModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Act
        var entity = await _genericFileAggregatorService.CreateItem(CreateCreateModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(Id)));

        // Assert
        AssertCreatedItem(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.UpdateItem(CreateUpdateModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Act
        var entity = await _genericFileAggregatorService.UpdateItem(CreateUpdateModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(Id)));

        // Assert
        AssertUpdatedItem(entity);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task DeleteItem_UserIdIsEmpty_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.DeleteItem(userId));
        Assert.Equal("Field validation failed, field name: userId, message: userId can't be empty.", exception.Message);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Act
        var id = await _genericFileAggregatorService.DeleteItem(Id);

        // Assert
        Assert.Equal(Id, id);
    }

    protected override void AssertExtractedItem(TGenericFileAggregatorModel model)
    {
        Assert.Equal(Id, model.Model.UserId);
        Assert.Equal(FileFieldName, model.Images.Single().FieldName);
        Assert.Equal(FileFieldContent, model.Images.Single().Value);
    }

    protected override void AssertCollection(TGenericFileAggregatorModel[] models)
    {
        Assert.All(models, m => Assert.Contains(m.Model.UserId, Ids));
        base.AssertCollection(models);
    }

    private Dictionary<string, object> CreateDefaultUpsertTestGenericFileAggregatorModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            },
            {
                "Images", CreateFileAggregatorImages()
            }
        };
    }
}