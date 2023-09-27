using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Common.UnitTests.Abstraction;
using FitnessApp.Comon.Tests.Shared;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Repository.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using Moq;
using Xunit;

namespace FitnessApp.Common.UnitTests.Abstractions.Db.Repository.Generic
{
    public class GenericRepositoryTests : TestBase
    {
        [Fact]
        public async Task GetAllItems_ReturnsAllItems()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var allEntities = TestData.GetAll(TestData.CreateGenericEntity, new Dictionary<string, object>());
            var queryableGenericEntitiesMock = GetQueryableMock(allEntities);
            dbContextMock
                .Setup(s => s.GetAllItems())
                .Returns(queryableGenericEntitiesMock.Object);

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var entities = await repository.GetAllItems();

            // Assert
            Assert.All(entities, i => Assert.NotNull(allEntities.Single(e => e.UserId == i.UserId)));
        }

        [Fact]
        public async Task GetItemByUserId_ReturnsSingleItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var queryableGenericEntitiesMock = GetQueryableGenericEntitiesMock();
            dbContextMock
               .Setup(s => s.GetItemById(It.IsAny<string>()))
               .ReturnsAsync(queryableGenericEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var entity = await repository.GetItemByUserId(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, entity.UserId);
        }

        [Fact]
        public async Task GetItemsByIds_ReturnsFilteredItems()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var queryableGenericEntitiesMock = GetQueryableGenericEntitiesMock();
            dbContextMock
                .Setup(s => s.GetItemsByIds(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(queryableGenericEntitiesMock.Object.Where(i => TestData.Ids.Contains(i.UserId)));

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var entities = await repository.GetItemsByIds(TestData.Ids);

            // Assert
            Assert.True(entities.Single().UserId == TestData.Id);
        }

        [Fact]
        public async Task CreateItem_ReturnsCreatedItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var createdEntity = TestData.CreateGenericEntity(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            dbContextMock
                .Setup(s => s.CreateItem(It.IsAny<TestGenericEntity>()))
                .ReturnsAsync(createdEntity);

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            var createModel = TestData.CreateCreateTestGenericModel(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );

            // Act
            var entity = await repository.CreateItem(createModel);

            // Assert
            Assert.Equal(createModel.UserId, entity.UserId);
            Assert.Equal(createModel.TestProperty1, entity.TestProperty1);
        }

        [Fact]
        public async Task UpdateItem_ReturnsUpdatedItem()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var queryableGenericEntitiesMock = GetQueryableGenericEntitiesMock();
            dbContextMock
               .Setup(s => s.GetItemById(It.IsAny<string>()))
               .ReturnsAsync(queryableGenericEntitiesMock.Object.Single(i => i.UserId == TestData.Id));

            var entityUpdate = TestData.CreateGenericEntity(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            entityUpdate.TestProperty1 = "Updated";

            dbContextMock
                .Setup(s => s.UpdateItem(It.IsAny<TestGenericEntity>()))
                .ReturnsAsync(entityUpdate);

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            var updateModel = TestData.CreateUpdateTestGenericModel(new Dictionary<string, object>
            {
                {
                    "Id", TestData.Id
                }
            });
            updateModel.TestProperty1 = "Updated";

            // Act
            var entity = await repository.UpdateItem(updateModel);

            // Assert
            Assert.Equal(updateModel.TestProperty1, entity.TestProperty1);
        }

        [Fact]
        public async Task DeleteItem_ReturnsDeletedItemId()
        {
            // Arrange
            var dbContextMock = new Mock<IDbContext<TestGenericEntity>>();
            var entity1 = TestData.CreateGenericEntity(
                new Dictionary<string, object>
                {
                    {
                        "Id", TestData.Id
                    }
                }
            );
            dbContextMock
                .Setup(s => s.DeleteItem(It.IsAny<string>()))
                .ReturnsAsync(entity1);

            var repository = new GenericRepositoryMock(dbContextMock.Object, _mapper);

            // Act
            var deletedId = await repository.DeleteItem(TestData.Id);

            // Assert
            Assert.Equal(TestData.Id, deletedId);
        }
    }
}