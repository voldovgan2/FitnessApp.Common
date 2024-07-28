using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.GenericFileAggregator;
using FitnessApp.Common.Files;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;
using Minio.Exceptions;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.GenericFileAggregator;

[Collection("GenericFileAggregatorService collection")]
public class GenericFileAggregatorServiceTest :
    IClassFixture<GenericFileAggregatorServiceGenericServiceFixture>,
    IClassFixture<GenericFileAggregatorServiceFileFixture>
{
    private readonly IGenericFileAggregatorService<
        TestGenericFileAggregatorModel,
        TestGenericModel,
        CreateTestGenericFileAggregatorModel,
        UpdateTestGenericFileAggregatorModel> _service;
    private readonly GenericFileAggregatorServiceFileFixture _fileFixture;

    public GenericFileAggregatorServiceTest(GenericFileAggregatorServiceGenericServiceFixture dbContextFixture, GenericFileAggregatorServiceFileFixture fileFixture)
    {
        _fileFixture = fileFixture;
        var genericFileAggregatorSettings = new GenericFileAggregatorSettings
        {
            ContainerName = fileFixture.Path,
            FileFields = new string[]
            {
                TestData.FileFieldName
            }
        };
        _service = new GenericFileAggregatorServiceMock(
            new GenericServiceMock(new GenericRepositoryMock(dbContextFixture.DbContext, dbContextFixture.Mapper)),
            _fileFixture.FilesService,
            dbContextFixture.Mapper,
            genericFileAggregatorSettings
        );
    }

    [Fact]
    public async Task GetItem_ReturnsSingleItem()
    {
        // Act
        var testGenericFileAggregatorModel = await _service.GetItemByUserId(TestData.EntityIdToGet);

        // Assert
        Assert.NotNull(testGenericFileAggregatorModel.Model.UserId);
        Assert.NotNull(testGenericFileAggregatorModel.Images.Single().FieldName);
        Assert.NotNull(testGenericFileAggregatorModel.Images.Single().Value);
    }

    [Fact]
    public async Task CreateItem_ReturnsCreated()
    {
        // Act
        var item = await _service.CreateItem(TestData.CreateCreateTestGenericFileAggregatorModel(CreateTestGenericFileAggregatorModelParams(TestData.EntityIdToCreate)));

        // Assert
        Assert.NotNull(item.Model.UserId);
        Assert.NotNull(item.Images.Single().FieldName);
        Assert.NotNull(item.Images.Single().Value);
    }

    [Fact]
    public async Task UpdateItem_ReturnsUpdated()
    {
        // Act
        var item = await _service.UpdateItem(TestData.CreateUpdateTestGenericFileAggregatorModel(CreateTestGenericFileAggregatorModelParams(TestData.EntityIdToUpdate)));

        // Assert
        Assert.NotNull(item.Model.UserId);
        Assert.NotNull(item.Images.Single().FieldName);
        Assert.NotNull(item.Images.Single().Value);
    }

    [Fact]
    public async Task DeleteItem_ReturnsDeleted()
    {
        var fileName = _fileFixture.FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToDelete);
        var fileBefore = await _fileFixture.FilesService.DownloadFile(_fileFixture.Path, fileName);
        Assert.NotNull(fileBefore);

        // Act
        var item = await _service.DeleteItem(TestData.EntityIdToDelete);

        // Assert
        Assert.Equal(TestData.EntityIdToDelete, item);
        await Assert.ThrowsAsync<ObjectNotFoundException>(() => _fileFixture.FilesService.DownloadFile(_fileFixture.Path, fileName));
    }

    private Dictionary<string, object> CreateTestGenericFileAggregatorModelParams(object id)
    {
        return new Dictionary<string, object>
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
