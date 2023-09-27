using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.GenericBlobAggregator;
using FitnessApp.Common.Blob;
using FitnessApp.Common.IntegrationTests.Abstraction.Services.Fixtures;
using FitnessApp.Common.IntegrationTests.Blob.Fixtures;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericBlobAggregator;
using Xunit;

namespace FitnessApp.Common.IntegrationTests.Abstraction.Services.GenericBlobAggregator
{
    [Collection("GenericBlobAggregatorService collection")]
    public class GenericBlobAggregatorServiceTest : IClassFixture<GenericBlobAggregatorServiceGenericServiceFixture>, IClassFixture<GenericBlobAggregatorServiceBlobFixture>
    {
        private readonly IGenericBlobAggregatorService<TestGenericEntity, TestGenericBlobAggregatorModel, TestGenericModel, CreateTestGenericBlobAggregatorModel, UpdateTestGenericBlobAggregatorModel> _service;
        private readonly GenericBlobAggregatorServiceBlobFixture _blobFixture;

        public GenericBlobAggregatorServiceTest(GenericBlobAggregatorServiceGenericServiceFixture dbContextFixture, GenericBlobAggregatorServiceBlobFixture blobFixture)
        {
            _blobFixture = blobFixture;
            var genericBlobAggregatorSettings = new GenericBlobAggregatorSettings
            {
                ContainerName = blobFixture.Path,
                BlobFields = new string[]
                {
                    TestData.BlobFieldName
                }
            };
            _service = new GenericBlobAggregatorServiceMock(
                new GenericServiceMock(
                    new GenericRepositoryMock(dbContextFixture.DbContext, dbContextFixture.Mapper),
                    dbContextFixture.SearchService,
                    dbContextFixture.Mapper
                ),
                blobFixture.BlobService,
                dbContextFixture.Mapper,
                genericBlobAggregatorSettings
            );
        }

        [Fact]
        public async Task GetItem_ReturnsSingleItem()
        {
            // Act
            var testGenericBlobAggregatorModel = await _service.GetItem(TestData.EntityIdToGet);

            // Assert
            Assert.NotNull(testGenericBlobAggregatorModel.Model.UserId);
            Assert.NotNull(testGenericBlobAggregatorModel.Images.Single().FieldName);
            Assert.NotNull(testGenericBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Act
            var item = await _service.CreateItem(TestData.CreateCreateTestGenericBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToCreate
                },
                {
                    "Images", TestData.CreateBlobAggregatorImages()
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
            var item = await _service.UpdateItem(TestData.CreateUpdateTestGenericBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.EntityIdToUpdate
                },
                {
                    "Images", TestData.CreateBlobAggregatorImages()
                }
            }));

            // Assert
            Assert.NotNull(item.Model.UserId);
            Assert.NotNull(item.Images.Single().FieldName);
            Assert.NotNull(item.Images.Single().Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsUpdated()
        {
            var blobName = BlobService.CreateBlobName(TestData.BlobFieldName, TestData.EntityIdToDelete);
            var bolbBefore = await _blobFixture.BlobService.DownloadFile(_blobFixture.Path, blobName);
            Assert.NotNull(bolbBefore);

            // Act
            var item = await _service.DeleteItem(TestData.EntityIdToDelete);

            // Assert
            Assert.Equal(TestData.EntityIdToDelete, item);
            var bolb = await _blobFixture.BlobService.DownloadFile(_blobFixture.Path, blobName);
            Assert.Null(bolb);
        }
    }
}
