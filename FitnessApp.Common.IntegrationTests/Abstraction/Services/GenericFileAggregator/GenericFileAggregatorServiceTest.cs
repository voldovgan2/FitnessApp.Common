using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.GenericFileAggregator;
using FitnessApp.Common.Files;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Common.IntegrationTests.File.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericFileAggregator;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.GenericFileAggregator
{
    [Collection("GenericFileAggregatorService collection")]
    public class GenericFileAggregatorServiceTest : IClassFixture<GenericFileAggregatorServiceGenericServiceFixture>, IClassFixture<GenericFileAggregatorServiceFileFixture>
    {
        private readonly IGenericFileAggregatorService<TestGenericEntity, TestGenericFileAggregatorModel, TestGenericModel, CreateTestGenericFileAggregatorModel, UpdateTestGenericFileAggregatorModel> _service;
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
                new GenericServiceMock(
                    new GenericRepositoryMock(dbContextFixture.DbContext, dbContextFixture.Mapper),
                    dbContextFixture.Mapper
                ),
                _fileFixture.FileService,
                dbContextFixture.Mapper,
                genericFileAggregatorSettings
            );
        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem()
        {
            // Act
            var testGenericFileAggregatorModel = await _service.GetItem(TestData.EntityIdToGet);

            // Assert
            Assert.NotNull(testGenericFileAggregatorModel.Model.UserId);
            Assert.NotNull(testGenericFileAggregatorModel.Images.Single().FieldName);
            Assert.NotNull(testGenericFileAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Act
            var item = await _service.CreateItem(TestData.CreateCreateTestGenericFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToCreate
                },
                {
                    "Images", TestData.CreateFileAggregatorImages()
                }
            }));

            // Assert
            Assert.NotNull(item.Model.UserId);
            Assert.NotNull(item.Images.Single().FieldName);
            Assert.NotNull(item.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsUpdated()
        {
            // Act
            var item = await _service.UpdateItem(TestData.CreateUpdateTestGenericFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "Images", TestData.CreateFileAggregatorImages()
                }
            }));

            // Assert
            Assert.NotNull(item.Model.UserId);
            Assert.NotNull(item.Images.Single().FieldName);
            Assert.NotNull(item.Images.Single().Value);
        }

        [Fact(Skip ="currently mocked")]
        public async Task DeleteItem_ReturnsDeleted()
        {
            var fileName = FilesService.CreateFileName(TestData.FileFieldName, TestData.EntityIdToDelete);
            var fileBefore = await _fileFixture.FileService.DownloadFile(_fileFixture.Path, fileName);
            Assert.NotNull(fileBefore);

            // Act
            var item = await _service.DeleteItem(TestData.EntityIdToDelete);

            // Assert
            Assert.Equal(TestData.EntityIdToDelete, item);
            var bolb = await _fileFixture.FileService.DownloadFile(_fileFixture.Path, fileName);
            Assert.Null(bolb);
        }
    }
}
