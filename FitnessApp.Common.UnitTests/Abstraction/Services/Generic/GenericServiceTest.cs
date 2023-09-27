using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Services.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstraction.Services.Generic
{
    public class GenericServiceTest : TestBase
    {
        [Fact]
        public async Task GetItemByUserId_ReturnsSingleItem()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();
            var queryableGenericModelsMock = GetQueryableMock(TestData.GetAll(TestData.CreateGenericModel, new Dictionary<string, object>()));
            repositoryMock
               .Setup(s => s.GetItemByUserId(It.IsAny<string>()))
               .ReturnsAsync(queryableGenericModelsMock.Object.Single(i => i.UserId == TestData.Id));

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            // Act
            var entity = await service.GetItemByUserId(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, entity.UserId);
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedBySearchCriteriaItems()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();
            var queryableGenericEntitiesMock = GetQueryableMock(TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>()));
            repositoryMock
               .Setup(s => s.GetAllItems())
               .ReturnsAsync(queryableGenericEntitiesMock.Object);
            var filteredBySearchItems = queryableGenericEntitiesMock.Object.Take(2).Select(i => i.UserId);
            searchMock
                .Setup(s => s.Search(It.IsAny<string>()))
                .ReturnsAsync(filteredBySearchItems);

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            var testProperty = "TestProperty1";

            // Act
            var models = await service.GetItems("", e => e.TestProperty1 == testProperty);

            // Assert
            Assert.All(models, m => filteredBySearchItems.Contains(m.UserId));
        }

        [Fact]
        public async Task GetItems_ReturnsMatchedByIdsItems()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();
            var queryableGenericEntitiesMock = GetQueryableMock(TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>()));
            repositoryMock
               .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
               .ReturnsAsync(queryableGenericEntitiesMock.Object.Where(e => TestData.Ids.Contains(e.UserId)));

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            // Act
            var models = await service.GetItems(TestData.Ids);

            // Assert
            Assert.All(models, m => Assert.Contains(m.UserId, TestData.Ids));
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedItem()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();

            var model = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            repositoryMock
                .Setup(s => s.CreateItem(It.IsAny<CreateTestGenericModel>()))
                .ReturnsAsync(model);

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            // Act
            var entity = await service.CreateItem(TestData.CreateCreateTestGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            ));

            // Assert
            Assert.Equal(TestData.Id, entity.UserId);
        }

        [Fact]
        public async Task UpdateItem_ReturnsUpdatedItem()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();
            var updateModel = TestData.CreateUpdateTestGenericModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                }
            });

            var model = TestData.CreateGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            model.TestProperty1 = updateModel.TestProperty1;

            repositoryMock
                .Setup(s => s.UpdateItem(It.IsAny<UpdateTestGenericModel>()))
                .ReturnsAsync(model);

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            // Act
            var entity = await service.UpdateItem(updateModel);

            // Assert
            Assert.Equal(model.TestProperty1, entity.TestProperty1);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItemId()
        {
            // Arrange
            var repositoryMock = new Mock<IGenericRepository<TestGenericEntity, TestGenericModel, CreateTestGenericModel, UpdateTestGenericModel>>();
            var searchMock = new Mock<ISearchService>();
            repositoryMock
                .Setup(s => s.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(TestData.Id);

            var service = new GenericServiceMock(repositoryMock.Object, searchMock.Object, _mapper);

            // Act
            var deletedId = await service.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, deletedId);
        }
    }
}