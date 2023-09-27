using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Blob;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.GenericBlobAggregator;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.GenericBlobAggregator
{
    public class GenericBlobAggregatorServiceTest : TestBase
    {
        [Fact]
        public async Task GetItem_ReturnsSingleItem()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
                .ReturnsAsync(genericModel);

            var blobContent = TestData.CreateBlobResult();
            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(blobContent);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var testGenericBlobAggregatorModel = await genericBlobAggregatorService.GetItem(TestData.Id);

            // Assert
            Assert.Equal(genericModel.UserId, testGenericBlobAggregatorModel.Model.UserId);
            Assert.Equal(genericModel.TestProperty1, testGenericBlobAggregatorModel.Model.TestProperty1);
            Assert.Equal(TestData.BlobFieldName, testGenericBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, testGenericBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedByPredicateItems()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.GetItems(It.IsAny<string>(), It.IsAny<Expression<Func<TestGenericEntity, bool>>>()))
                .ReturnsAsync(new List<TestGenericModel> { genericModel });

            var blobContent = TestData.CreateBlobResult();
            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(blobContent);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var testGenericBlobAggregatorModels = await genericBlobAggregatorService.GetItems("", e => e.UserId == TestData.Id);

            // Assert
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(genericModel.UserId, testGenericBlobAggregatorModel.Model.UserId));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericBlobAggregatorModel.Model.TestProperty1));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(TestData.BlobFieldName, testGenericBlobAggregatorModel.Images.Single().FieldName));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(TestData.BlobFieldContent, testGenericBlobAggregatorModel.Images.Single().Value));
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedByIdsItems()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.GetItems(It.IsAny<IEnumerable<string>>()))
               .ReturnsAsync(new List<TestGenericModel> { genericModel });

            var blobContent = TestData.CreateBlobResult();
            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(blobContent);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var testGenericBlobAggregatorModels = await genericBlobAggregatorService.GetItems(TestData.Ids);

            // Assert
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(genericModel.UserId, testGenericBlobAggregatorModel.Model.UserId));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericBlobAggregatorModel.Model.TestProperty1));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(TestData.BlobFieldName, testGenericBlobAggregatorModel.Images.Single().FieldName));
            Assert.All(testGenericBlobAggregatorModels, testGenericBlobAggregatorModel => Assert.Equal(TestData.BlobFieldContent, testGenericBlobAggregatorModel.Images.Single().Value));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedItem()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            var createGenericBlobAggregatorModel = TestData.CreateCreateTestGenericBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Images", TestData.CreateBlobAggregatorImages()
                }
            });

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
                .ReturnsAsync(genericModel);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var testGenericBlobAggregatorModel = await genericBlobAggregatorService.CreateItem(createGenericBlobAggregatorModel);

            // Assert
            Assert.Equal(genericModel.UserId, testGenericBlobAggregatorModel.Model.UserId);
            Assert.Equal(genericModel.TestProperty1, testGenericBlobAggregatorModel.Model.TestProperty1);
            Assert.Equal(TestData.BlobFieldName, testGenericBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, testGenericBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItem_ReturnsUpdatedItem()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            genericModel.TestProperty1 = "TestProperty1Updated";

            var updateGenericBlobAggregatorModel = TestData.CreateUpdateTestGenericBlobAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Images", TestData.CreateBlobAggregatorImages()
                }
            });

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
               .ReturnsAsync(genericModel);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);
            blobService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var testGenericBlobAggregatorModel = await genericBlobAggregatorService.UpdateItem(updateGenericBlobAggregatorModel);

            // Assert
            Assert.Equal(genericModel.UserId, testGenericBlobAggregatorModel.Model.UserId);
            Assert.Equal(genericModel.TestProperty1, testGenericBlobAggregatorModel.Model.TestProperty1);
            Assert.Equal(TestData.BlobFieldName, testGenericBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, testGenericBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItemId()
        {
            // Arrange
            var genericBlobAggregatorSettings = TestData.CreateGenericBlobAggregatorSettings();

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(TestData.Id);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var genericBlobAggregatorService = new GenericBlobAggregatorServiceMock(
                genericService.Object,
                blobService.Object,
                _mapper,
                genericBlobAggregatorSettings
            );

            // Act
            var id = await genericBlobAggregatorService.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, id);
        }
    }
}