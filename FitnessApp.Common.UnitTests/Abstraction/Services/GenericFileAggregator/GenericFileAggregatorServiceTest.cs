using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.GenericFileAggregator;

public class GenericFileAggregatorServiceTest : TestBase
{
    private readonly Mock<
        IGenericService<
            TestGenericEntity,
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel>
        > _genericService;
    private readonly Mock<IFilesService> _fileService;
    private readonly GenericFileAggregatorServiceMock _genericFileAggregatorService;
    private readonly Dictionary<string, object> _defaultGenericModelParameters = new()
    {
        {
            "Id", TestData.Id
        }
    };
    private readonly Dictionary<string, object> _defaultCreateTestGenericFileAggregatorModelParameters = new()
    {
        {
            "Id", TestData.Id
        },
        {
            "Images", TestData.CreateFileAggregatorImages()
        }
    };

    public GenericFileAggregatorServiceTest()
    {
        _genericService = new Mock<
            IGenericService<
                TestGenericEntity,
                TestGenericModel,
                CreateTestGenericModel,
                UpdateTestGenericModel>>();
        _fileService = new Mock<IFilesService>();
        _genericFileAggregatorService = new GenericFileAggregatorServiceMock(
            _genericService.Object,
            _fileService.Object,
            _mapper,
            TestData.CreateGenericFileAggregatorSettings()
        );
    }

    [Fact]
    public async Task GetItemByUserId_ReturnsSingleItem()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(_defaultGenericModelParameters);

        _genericService
            .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
            .ReturnsAsync(genericModel);

        var fileContent = TestData.CreateFileResult();
        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(fileContent);

        // Act
        var testGenericFileAggregatorModel = await _genericFileAggregatorService.GetItemByUserId(TestData.Id);

        // Assert
        Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId);
        Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1);
        Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task FilterItems_ReturnsMatchedByPredicateItems()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(_defaultGenericModelParameters);

        _genericService
            .Setup(s => s.FilterItems(It.IsAny<Expression<Func<TestGenericEntity, bool>>>()))
            .ReturnsAsync(new List<TestGenericModel> { genericModel });

        var fileContent = TestData.CreateFileResult();
        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(fileContent);

        // Act
        var testGenericFileAggregatorModels = await _genericFileAggregatorService.FilterItems(e => e.UserId == TestData.Id);

        // Assert
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value));
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsItems()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(_defaultGenericModelParameters);

        _genericService
            .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
           .ReturnsAsync(new List<TestGenericModel> { genericModel });

        var fileContent = TestData.CreateFileResult();
        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(fileContent);

        // Act
        var testGenericFileAggregatorModels = await _genericFileAggregatorService.GetItemsByIds(TestData.Ids);

        // Assert
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value));
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(_defaultGenericModelParameters);

        var createGenericFileAggregatorModel = TestData.CreateCreateTestGenericFileAggregatorModel(_defaultCreateTestGenericFileAggregatorModelParameters);

        _genericService
            .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
            .ReturnsAsync(genericModel);

        _fileService
            .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);

        // Act
        var testGenericFileAggregatorModel = await _genericFileAggregatorService.CreateItem(createGenericFileAggregatorModel);

        // Assert
        Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId);
        Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1);
        Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(_defaultGenericModelParameters);
        genericModel.TestProperty1 = "TestProperty1Updated";

        var updateGenericFileAggregatorModel = TestData.CreateUpdateTestGenericFileAggregatorModel(_defaultCreateTestGenericFileAggregatorModelParameters);

        _genericService
            .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
           .ReturnsAsync(genericModel);

        _fileService
            .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .Returns(Task.CompletedTask);
        _fileService
            .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var testGenericFileAggregatorModel = await _genericFileAggregatorService.UpdateItem(updateGenericFileAggregatorModel);

        // Assert
        Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId);
        Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1);
        Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName);
        Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeletedItemId()
    {
        // Arrange
        _genericService
            .Setup(s => s.DeleteItem(It.IsAny<string>()))
            .ReturnsAsync(TestData.Id);

        _fileService
            .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var id = await _genericFileAggregatorService.DeleteItem(TestData.Id);

        // Assert
        Assert.Equal(TestData.Id, id);
    }
}