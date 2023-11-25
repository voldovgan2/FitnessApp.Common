using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Files;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionFileAggregatorServiceMock;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.CollectionFileAggregator
{
    public class CollectionFileAggregatorServiceTest : TestBase
    {
        [Fact]
        public async Task GetItemByUserId_ReturnsSingleItem()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var collectionModel = TestData.CreateCollectionModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    },
                    {
                        "ItemsCount", 2
                    }
                }
            );

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
                .ReturnsAsync(collectionModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateFileResult());

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            // Act
            var testCollectionFileAggregatorModel = await collectionFilesAggregatorService.GetItemByUserId(TestData.Id);

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
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var collectionItems = new List<TestCollectionItemModel>
            {
                TestData.CreateCollectionItemModel(1),
                TestData.CreateCollectionItemModel(2)
            };

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(collectionItems);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateFileResult());

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            var getFilteredCollectionItemsModel = new GetTestFilteredCollectionItemsModel
            {
                UserId = TestData.Id,
                Predicate = i => i.TestProperty.Contains("Property"),
                CollectionName = TestData.CollectionName,
                Page = 0,
                PageSize = 10
            };

            // Act
            var testCollectionFileAggregatorItemModel = await collectionFilesAggregatorService.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.NotNull(collectionItems.Single(i => i.Id == item.Model.Id)));
            Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.Equal(TestData.FileFieldName, item.Images.Single().FieldName));
            Assert.All(testCollectionFileAggregatorItemModel.Items, item => Assert.Equal(TestData.FileFieldContent, item.Images.Single().Value));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.CreateItem(It.IsAny<CreateTestCollectionModel>()))
                .ReturnsAsync(TestData.Id);

            var fileService = new Mock<IFilesService>();

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            var ecreateCollectionFilAggregatorModel = TestData.CreateCreateTestCollectionFileAggregatorModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            // Act
            var id = await collectionFilesAggregatorService.CreateItem(ecreateCollectionFilAggregatorModel);

            // Assert
            Assert.Equal(TestData.Id, id);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var addItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(addItemModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateFileResult());

            fileService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Add
                },
                {
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionFileAggregatorModel = await collectionFilesAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.Equal(addItemModel.Id, collectionFileAggregatorModel.Model.Id);
            Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var updateItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(updateItemModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateFileResult());

            fileService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            fileService
               .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Update
                },
                {
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionFileAggregatorModel = await collectionFilesAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.Equal(updateItemModel.Id, collectionFileAggregatorModel.Model.Id);
            Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemRemoveCollectionItem_ReturnsRemovedItem()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var removeItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(removeItemModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateFileResult());

            fileService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            fileService
               .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            var collectionFileAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            var updateCollectionFileAggregatorModel = TestData.CreateUpdateTestCollectionFileAggregatorModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                },
                {
                    "CollectionName", TestData.CollectionName
                },
                {
                    "Action", UpdateCollectionAction.Remove
                },
                {
                    "Model", TestData.CreateTestCollectionFileAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionFileAggregatorModel = await collectionFileAggregatorService.UpdateItem(updateCollectionFileAggregatorModel);

            // Assert
            Assert.Equal(removeItemModel.Id, collectionFileAggregatorModel.Model.Id);
            Assert.Equal(TestData.FileFieldName, collectionFileAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.FileFieldContent, collectionFileAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItem()
        {
            // Arrange
            var collectionFileAggregatorSettings = TestData.CreateCollectionFileAggregatorSettings();

            var removedCollectionModel = TestData.CreateCollectionModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", 1
                    },
                    {
                        "ItemsCount", 2
                    }
                }
            );

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(removedCollectionModel);

            var fileService = new Mock<IFilesService>();
            fileService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var collectionFilesAggregatorService = new CollectionFileAggregatorServiceMock(
                collectionService.Object,
                fileService.Object,
                _mapper,
                collectionFileAggregatorSettings
            );

            // Act
            var deletedId = await collectionFilesAggregatorService.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(removedCollectionModel.UserId, deletedId);
        }
    }
}