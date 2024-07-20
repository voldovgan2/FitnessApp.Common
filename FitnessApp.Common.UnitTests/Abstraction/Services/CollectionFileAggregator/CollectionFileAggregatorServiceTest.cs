using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionFileAggregatorServiceMock;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.CollectionFileAggregator;

public class CollectionFileAggregatorServiceTest : TestBase
{
    private readonly Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>> _collectionService;
    private readonly CollectionFileAggregatorServiceMock _collectionFilesAggregatorService;
    private readonly Mock<IFilesService> _fileService;
    private readonly Dictionary<string, object> _defaultTestCollectionEntityParameters = new()
    {
        {
            "Id", TestData.Id
        },
        {
            "ItemsCount", 2
        }
    };

    public CollectionFileAggregatorServiceTest()
    {
        _collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
        _fileService = new Mock<IFilesService>();
        _collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
            _collectionService.Object,
            _fileService.Object,
            _mapper,
            TestData.CreateCollectionFileAggregatorSettings()
        );
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Arrange
        var collectionModel = TestData.CreateCollectionModel(_defaultTestCollectionEntityParameters);

        _collectionService
            .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
            .ReturnsAsync(collectionModel);

        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.CreateFileResult());

        // Act
        var testCollectionFileAggregatorModel = await _collectionFilesAggregatorService.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(testCollectionFileAggregatorModel.UserId, collectionModel.UserId);
        Assert.All(testCollectionFileAggregatorModel.Collection[TestData.CollectionName], item => Assert.NotNull(collectionModel.Collection[TestData.CollectionName].Single(i => i.Id == item.Model.Id)));
        Assert.All(testCollectionFileAggregatorModel.Collection[TestData.CollectionName], item => Assert.Equal(TestData.FileFieldName, item.Images.Single().FieldName));
        Assert.All(testCollectionFileAggregatorModel.Collection[TestData.CollectionName], item => Assert.Equal(TestData.FileFieldContent, item.Images.Single().Value));
    }

    [Fact]
    public async Task GetFilteredCollectionItems_ReturnsMatchedItem()
    {
        // Arrange
        var collectionItems = new List<TestCollectionItemModel>
        {
            TestData.CreateCollectionItemModel(1),
            TestData.CreateCollectionItemModel(2)
        };

        _collectionService
            .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(collectionItems);

        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.CreateFileResult());

        var getFilteredCollectionItemsModel = new GetTestFilteredCollectionItemsModel
        {
            UserId = TestData.Id,
            Predicate = i => i.TestProperty.Contains("Property"),
            CollectionName = TestData.CollectionName,
            Page = 0,
            PageSize = 10
        };

        // Act
        var testCollectionFileAggregatorItemModel = await _collectionFilesAggregatorService.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

        // Assert
        Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.NotNull(collectionItems.Single(i => i.Id == item.Model.Id)));
        Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.Equal(TestData.FileFieldName, item.Images.Single().FieldName));
        Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.Equal(TestData.FileFieldContent, item.Images.Single().Value));
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
        // Arrange
        _collectionService
            .Setup(s => s.CreateItem(It.IsAny<CreateTestCollectionModel>()))
            .ReturnsAsync(TestData.Id);

        var ecreateCollectionFilAggregatorModel = TestData.CreateCreateTestCollectionFileAggregatorModel(
            new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                }
            }
        );

        // Act
        var id = await _collectionFilesAggregatorService.CreateItem(ecreateCollectionFilAggregatorModel);

        // Assert
        Assert.Equal(TestData.Id, id);
    }

    [Fact]
    public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
    {
        // Arrange
        var addItemModel = TestData.CreateCollectionItemModel(1);

        _collectionService
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
            .ReturnsAsync(addItemModel);

        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.CreateFileResult());

        _fileService
            .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        var updateCollectionFileAggregatorModel = CreateUpdateTestCollectionFileAggregatorModel(UpdateCollectionAction.Add);

        // Act
        var collectionFileAggregatorModel = await _collectionFilesAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

        // Assert
        Assert.Equal(addItemModel.Id, collectionFileAggregatorModel.Model.Id);
        Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
    {
        // Arrange
        var updateItemModel = TestData.CreateCollectionItemModel(1);

        _collectionService
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
            .ReturnsAsync(updateItemModel);

        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.CreateFileResult());

        _fileService
            .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        _fileService
           .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
           .Returns(Task.CompletedTask);

        var updateCollectionFileAggregatorModel = CreateUpdateTestCollectionFileAggregatorModel(UpdateCollectionAction.Update);

        // Act
        var collectionFileAggregatorModel = await _collectionFilesAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

        // Assert
        Assert.Equal(updateItemModel.Id, collectionFileAggregatorModel.Model.Id);
        Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task UpdateItemRemoveCollectionItem_ReturnsRemovedItem()
    {
        // Arrange
        var removeItemModel = TestData.CreateCollectionItemModel(1);

        _collectionService
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
            .ReturnsAsync(removeItemModel);

        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(TestData.CreateFileResult());

        _fileService
            .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        _fileService
           .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
           .Returns(Task.CompletedTask);

        var updateCollectionFileAggregatorModel = CreateUpdateTestCollectionFileAggregatorModel(UpdateCollectionAction.Remove);

        // Act
        var collectionFileAggregatorModel = await _collectionFilesAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

        // Assert
        Assert.Equal(removeItemModel.Id, collectionFileAggregatorModel.Model.Id);
        Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItem()
    {
        // Arrange
        var removedCollectionModel = TestData.CreateCollectionModel(_defaultTestCollectionEntityParameters);

        _collectionService
            .Setup(s => s.DeleteItem(It.IsAny<string>()))
            .ReturnsAsync(removedCollectionModel);
        _fileService
            .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var deletedId = await _collectionFilesAggregatorService.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(removedCollectionModel.UserId, deletedId);
    }

    private UpdateTestCollectionFileAggregatorModel CreateUpdateTestCollectionFileAggregatorModel(UpdateCollectionAction action)
    {
        return TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
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
                "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>())
            },
        });
    }
}