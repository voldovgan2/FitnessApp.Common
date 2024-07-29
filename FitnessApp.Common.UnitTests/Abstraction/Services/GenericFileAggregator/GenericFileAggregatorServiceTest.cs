using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Exceptions;
using FitnessApp.Common.Files;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.GenericFileAggregator;

public class GenericFileAggregatorServiceTest : TestBase
{
    private readonly Mock<
        IGenericService<
            TestGenericModel,
            CreateTestGenericModel,
            UpdateTestGenericModel>
        > _genericService;
    private readonly Mock<IFilesService> _fileService;
    private readonly GenericFileAggregatorServiceMock _genericFileAggregatorService;

    public GenericFileAggregatorServiceTest()
    {
        _genericService = new Mock<
            IGenericService<
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
        // Arrange
        var genericModel = TestData.CreateGenericModel(CreateDefaultGenericModelParameters(TestData.Id));

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
    public async Task GetItemByIds_IdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.GetItemsByIds([]));
        Assert.Equal("Field validation failed, field name: ids, message: ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsItems()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(CreateDefaultGenericModelParameters(TestData.Id));

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
    public async Task GetItemByIds_PagedIdsIsEmpty_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _genericFileAggregatorService.GetItemsByIds(new GetTestPagedByIdsDataModel
        {
            Ids = Enumerable.Empty<string>(),
            Page = 1,
            PageSize = 1,
        }));
        Assert.Equal("Field validation failed, field name: Ids, message: Ids can't be empty.", exception.Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageLessThen0_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.GetItemsByIds(new GetTestPagedByIdsDataModel
        {
            Ids = TestData.Ids,
            Page = -1,
            PageSize = 1,
        }));
        Assert.Equal("Field validation failed, field name: Page, message: Page should be within the range [0, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemByIds_PagedPageSizeLessThen1_ThrowsValidationError()
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.GetItemsByIds(new GetTestPagedByIdsDataModel
        {
            Ids = TestData.Ids,
            Page = 0,
            PageSize = 0,
        }));
        Assert.Equal("Field validation failed, field name: PageSize, message: PageSize should be within the range [1, 2147483647].", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task GetItemsByIds_ReturnsMatchedByIdsPagedItems()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(CreateDefaultGenericModelParameters(TestData.Id));
        var model = new GetTestPagedByIdsDataModel
        {
            Ids = TestData.Ids,
            Page = 1,
            PageSize = 1,
        };

        _genericService
            .Setup(s => s.GetItemsByIds(It.IsAny<GetTestPagedByIdsDataModel>()))
            .ReturnsAsync(new List<TestGenericModel> { genericModel }.ToPaged(model));

        var fileContent = TestData.CreateFileResult();
        _fileService
            .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(fileContent);

        // Act
        var testGenericFileAggregatorModels = (await _genericFileAggregatorService.GetItemsByIds(model)).Items;

        // Assert
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName));
        Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task CreateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.CreateItem(TestData.CreateCreateTestGenericFileAggregatorModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreatedItem()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(CreateDefaultGenericModelParameters(TestData.Id));

        var createGenericFileAggregatorModel = TestData.CreateCreateTestGenericFileAggregatorModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(TestData.Id));

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

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UpdateItem_EmptyUserId_ThrowsValidationError(string userId)
    {
        // Assert
        var exception = await Assert.ThrowsAsync<AggregateException>(() => _genericFileAggregatorService.UpdateItem(TestData.CreateUpdateTestGenericFileAggregatorModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(userId))));
        Assert.Equal("Field validation failed, field name: UserId, message: UserId can't be empty.", exception.InnerExceptions.Single().Message);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdatedItem()
    {
        // Arrange
        var genericModel = TestData.CreateGenericModel(CreateDefaultGenericModelParameters(TestData.Id));
        genericModel.TestProperty1 = "TestProperty1Updated";

        var updateGenericFileAggregatorModel = TestData.CreateUpdateTestGenericFileAggregatorModel(CreateDefaultUpsertTestGenericFileAggregatorModelParameters(TestData.Id));

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

    private Dictionary<string, object> CreateDefaultGenericModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            }
        };
    }

    private Dictionary<string, object> CreateDefaultUpsertTestGenericFileAggregatorModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            },
            {
                "Images", TestData.CreateFileAggregatorImages()
            }
        };
    }
}