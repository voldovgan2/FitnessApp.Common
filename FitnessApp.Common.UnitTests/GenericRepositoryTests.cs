using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Abstractions.Models;
using Xunit;

namespace FitnessApp.Common.UnitTests;

public abstract class GenericRepositoryTests<
    TGenericEntity,
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel> :
    TestBase<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericEntity : IWithUserIdEntity
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    protected readonly GenericRepository<
        TGenericEntity,
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel> Repository;

    protected GenericRepositoryTests(
        GenericRepository<
            TGenericEntity,
            TGenericModel,
            TCreateGenericModel,
            TUpdateGenericModel> repository)
    {
        Repository = repository;
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Act
        var entity = await Repository.GetItemByUserId(Id);

        // Assert
        AssertExtractedItem(entity);
    }

    [Fact]
    public async Task GetItemByIds_ReturnsMathcedItems()
    {
        // Act
        var models = await Repository.GetItemByUserIds([Id]);

        // Assert
        AssertCollection(models);
    }

    [Fact]
    public async Task GetItems_ReturnsMathcedItems()
    {
        // Act
        var model = await Repository.GetItems(CreateGetPagedByIdsDataModel([Id], 0, 10));

        // Assert
        AssertCollection(model.Items);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Act
        var entity = await Repository.CreateItem(CreateCreateModel(CreateGenericModelParameters(Id)));

        // Assert
        AssertCreatedItem(entity);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Act
        var entity = await Repository.UpdateItem(CreateUpdateModel(CreateGenericModelParameters(Id)));

        // Assert
        AssertUpdatedItem(entity);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Act
        var deletedId = await Repository.DeleteItem(Id);

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