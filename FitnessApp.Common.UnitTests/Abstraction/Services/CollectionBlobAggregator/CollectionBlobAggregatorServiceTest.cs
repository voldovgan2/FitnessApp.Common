using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Abstractions.Services.CollectionBlobAggregator;
using FitnessApp.Common.Blob;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.CollectionBlobAggregatorServiceMock;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.CollectionBlobAggregator
{
    public class CollectionBlobAggregatorServiceTest : TestBase
    {
        [Fact]
        public async Task GetItemByUserId_ReturnsSingleItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

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

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateBlobResult());

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            // Act
            var testCollectionBlobAggregatorModel = await collectionBlobAggregatorService.GetItemByUserId(TestData.Id);

            // Assert
            Assert.Equal(testCollectionBlobAggregatorModel.UserId, collectionModel.UserId);
            Assert.All(testCollectionBlobAggregatorModel.Collection[TestData.CollectionName], item => Assert.NotNull(collectionModel.Collection[TestData.CollectionName].Single(i => i.Id == item.Model.Id)));
            Assert.All(testCollectionBlobAggregatorModel.Collection[TestData.CollectionName], item => Assert.Equal(TestData.BlobFieldName, item.Images.Single().FieldName));
            Assert.All(testCollectionBlobAggregatorModel.Collection[TestData.CollectionName], item => Assert.Equal(TestData.BlobFieldContent, item.Images.Single().Value));
        }

        [Fact]
        public async Task GetFilteredCollectionItems_ReturnsMatchedItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

            var collectionItems = new List<TestCollectionItemModel>
            {
                TestData.CreateCollectionItemModel(1),
                TestData.CreateCollectionItemModel(2)
            };

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.GetCollectionByUserId(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(collectionItems);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateBlobResult());

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
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
            var testCollectionBlobAggregatorItemModel = await collectionBlobAggregatorService.GetFilteredCollectionItems(getFilteredCollectionItemsModel);

            // Assert
            Assert.All(testCollectionBlobAggregatorItemModel.Items, item => Assert.NotNull(collectionItems.Single(i => i.Id == item.Model.Id)));
            Assert.All(testCollectionBlobAggregatorItemModel.Items, item => Assert.Equal(TestData.BlobFieldName, item.Images.Single().FieldName));
            Assert.All(testCollectionBlobAggregatorItemModel.Items, item => Assert.Equal(TestData.BlobFieldContent, item.Images.Single().Value));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreated()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.CreateItem(It.IsAny<CreateTestCollectionModel>()))
                .ReturnsAsync(TestData.Id);

            var blobService = new Mock<IBlobService>();

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            var createCollectionBlobAggregatorModel = TestData.CreateCreateTestCollectionBlobAggregatorModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            // Act
            var id = await collectionBlobAggregatorService.CreateItem(createCollectionBlobAggregatorModel);

            // Assert
            Assert.Equal(TestData.Id, id);
        }

        [Fact]
        public async Task UpdateItemAddCollectionItem_ReturnsAddedItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

            var addItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(addItemModel);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateBlobResult());

            blobService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionBlobAggregatorModel = await collectionBlobAggregatorService.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.Equal(addItemModel.Id, collectionBlobAggregatorModel.Model.Id);
            Assert.Equal(TestData.BlobFieldName, collectionBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, collectionBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemUpdateCollectionItem_ReturnsUpdatedItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

            var updateItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(updateItemModel);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateBlobResult());

            blobService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            blobService
               .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionBlobAggregatorModel = await collectionBlobAggregatorService.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.Equal(updateItemModel.Id, collectionBlobAggregatorModel.Model.Id);
            Assert.Equal(TestData.BlobFieldName, collectionBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, collectionBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task UpdateItemRemoveCollectionItem_ReturnsRemovedItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

            var removeItemModel = TestData.CreateCollectionItemModel(1);

            var collectionService = new Mock<ICollectionService<TestCollectionModel, TestCollectionItemModel, CreateTestCollectionModel, UpdateTestCollectionModel>>();
            collectionService
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestCollectionModel>()))
                .ReturnsAsync(removeItemModel);

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DownloadFile(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(TestData.CreateBlobResult());

            blobService
                .Setup(s => s.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            blobService
               .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
               .Returns(Task.CompletedTask);

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            var updateCollectionBlobAggregatorModel = TestData.CreateUpdateTestCollectionBlobAggregatorModel(new Dictionary<string, object>
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
                    "Model", TestData.CreateTestCollectionBlobAggregatorItemModel(new Dictionary<string, object>())
                }
            });

            // Act
            var collectionBlobAggregatorModel = await collectionBlobAggregatorService.UpdateItem(updateCollectionBlobAggregatorModel);

            // Assert
            Assert.Equal(removeItemModel.Id, collectionBlobAggregatorModel.Model.Id);
            Assert.Equal(TestData.BlobFieldName, collectionBlobAggregatorModel.Images.Single().FieldName);
            Assert.Equal(TestData.BlobFieldContent, collectionBlobAggregatorModel.Images.Single().Value);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItem()
        {
            // Arrange
            var collectionBlobAggregatorSettings = TestData.CreateCollectionBlobAggregatorSettings();

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

            var blobService = new Mock<IBlobService>();
            blobService
                .Setup(s => s.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var collectionBlobAggregatorService = new CollectionBlobAggregatorServiceMock(
                collectionService.Object,
                blobService.Object,
                _mapper,
                collectionBlobAggregatorSettings
            );

            // Act
            var deletedId = await collectionBlobAggregatorService.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(removedCollectionModel.UserId, deletedId);
        }
    }
}