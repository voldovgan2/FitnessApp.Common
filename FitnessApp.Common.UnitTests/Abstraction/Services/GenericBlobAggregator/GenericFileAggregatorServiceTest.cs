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

namespace FitnessApp.Common.UnitTests.Abstraction.Services.GenericFileAggregator
{
    public class GenericFileAggregatorServiceTest : TestBase
    {
        [Fact]
        public async Task GetItem_ReturnsSingleItem()
        {
            // Arrange
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

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

            var fileContent = TestData.CreateFileResult();
            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileContent);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var testGenericFileAggregatorModel = await genericFileAggregatorService.GetItem(TestData.Id);

            // Assert
            Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId);
            Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1);
            Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedByPredicateItems()
        {
            // Arrange
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

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

            var fileContent = TestData.CreateFileResult();
            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileContent);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var testGenericFileAggregatorModels = await genericFileAggregatorService.GetItems("", e => e.UserId == TestData.Id);

            // Assert
            Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.UserId, testGenericFileAggregatorModel.Model.UserId));
            Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(genericModel.TestProperty1, testGenericFileAggregatorModel.Model.TestProperty1));
            Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldName, testGenericFileAggregatorModel.Images.Single().FieldName));
            Assert.All(testGenericFileAggregatorModels, testGenericFileAggregatorModel => Assert.Equal(TestData.FileFieldContent, testGenericFileAggregatorModel.Images.Single().Value));
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedByIdsItems()
        {
            // Arrange
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

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

            var fileContent = TestData.CreateFileResult();
            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileContent);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var testGenericFileAggregatorModels = await genericFileAggregatorService.GetItems(TestData.Ids);

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
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            var createGenericFileAggregatorModel = TestData.CreateCreateTestGenericFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Images", TestData.CreateFileAggregatorImages()
                }
            });

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
                .ReturnsAsync(genericModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var testGenericFileAggregatorModel = await genericFileAggregatorService.CreateItem(createGenericFileAggregatorModel);

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
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

            var genericModel = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            genericModel.TestProperty1 = "TestProperty1Updated";

            var updateGenericFileAggregatorModel = TestData.CreateUpdateTestGenericFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "Images", TestData.CreateFileAggregatorImages()
                }
            });

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
               .ReturnsAsync(genericModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);
            fileService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var testGenericFileAggregatorModel = await genericFileAggregatorService.UpdateItem(updateGenericFileAggregatorModel);

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
            var genericFileAggregatorSettings = TestData.CreateGenericFileAggregatorSettings();

            var genericService = new Mock<IGenericService<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            genericService
                .Setup(s => s.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(TestData.Id);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var genericFileAggregatorService = new GenericFileAggregatorServiceMock(
                genericService.Object,
                fileService.Object,
                _mapper,
                genericFileAggregatorSettings
            );

            // Act
            var id = await genericFileAggregatorService.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, id);
        }
    }
}