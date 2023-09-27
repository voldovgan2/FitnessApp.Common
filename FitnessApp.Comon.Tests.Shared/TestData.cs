using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionBlobAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericBlobAggregator;

namespace FitnessApp.Comon.Tests.Shared
{
    public static class TestData
    {
        public static string EntityIdToGet { get; } = "EntityIdToGet";
        public static string EntityIdToCreate { get; } = "EntityIdToCreate";
        public static string EntityIdToUpdate { get; } = "EntityIdToUpdate";
        public static string EntityIdToDelete { get; } = "EntityIdToDelete";

        public static string BlobToDownload { get; } = "BlobToDownload";
        public static string BlobToUpload { get; } = "BlobToUpload";
        public static string BlobToDelete { get; } = "BlobToDelete";

        public static string Id { get; } = "1";
        public static string[] Ids { get; } = new string[] { Id };
        public static string CollectionName { get; } = "Collection";
        public static string ContainerName { get; } = "ContainerName";
        public static string BlobFieldName { get; } = "BlobField";
        public static string BlobFieldContent { get; } = "BlobFieldContent";

        private static string _propertyPrefix = "TestProperty";

        #region Abstract

        public static List<T> GetAll<T>(Func<Dictionary<string, object>, T> createElementFactory, Dictionary<string, object> args)
            where T : class
        {
            var result = new List<T>();
            args.Add("Id", 0);

            for (int k = 0; k < 4; k++)
            {
                args["Id"] = k + 1;
                result.Add(createElementFactory(args));
            }

            return result;
        }

        #endregion

        #region Generic initialization

        public static TestGenericEntity CreateGenericEntity(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new TestGenericEntity
            {
                UserId = id.ToString(),
                TestProperty1 = $"{_propertyPrefix}{id}"
            };
        }

        public static CreateTestGenericModel CreateCreateTestGenericModel(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new CreateTestGenericModel
            {
                UserId = id.ToString(),
                TestProperty1 = $"{_propertyPrefix}{id}"
            };
        }

        public static UpdateTestGenericModel CreateUpdateTestGenericModel(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new UpdateTestGenericModel
            {
                UserId = id.ToString(),
                TestProperty1 = $"{_propertyPrefix}{id}"
            };
        }

        public static TestGenericModel CreateGenericModel(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new TestGenericModel
            {
                UserId = id.ToString(),
                TestProperty1 = $"{_propertyPrefix}{id}"
            };
        }

        #endregion

        #region Collection initialization

        public static TestCollectionItemModel CreateCollectionItemModel(object index, string suffix = "")
        {
            return new TestCollectionItemModel
            {
                Id = index.ToString(),
                TestProperty = $"{_propertyPrefix}{index}{suffix}"
            };
        }

        public static TestCollectionEntity CreateCollectionEntity(Dictionary<string, object> args)
        {
            var collectionItems = new List<ICollectionItemEntity>();
            var itemsCount = (int)args["ItemsCount"];
            for (int k = 0; k < itemsCount; k++)
            {
                collectionItems.Add(
                    new TestCollectionItemEntity
                    {
                        Id = k.ToString(),
                        TestProperty = $"{_propertyPrefix}{k}"
                    }
                );
            }

            return new TestCollectionEntity
            {
                UserId = args["Id"].ToString(),
                Collection = new Dictionary<string, List<ICollectionItemEntity>>
                {
                    {
                        CollectionName,
                        collectionItems
                    }
                }
            };
        }

        public static CreateTestCollectionModel CreateCreateTestCollectionModel(Dictionary<string, object> args)
        {
            return new CreateTestCollectionModel
            {
                UserId = args["Id"].ToString(),
                Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>()
            };
        }

        public static UpdateTestCollectionModel CreateUpdateTestCollectionModel(Dictionary<string, object> args)
        {
            return new UpdateTestCollectionModel
            {
                UserId = args["Id"].ToString(),
                CollectionName = args["CollectionName"].ToString(),
                Action = (UpdateCollectionAction)args["Action"],
                Model = (ICollectionItemModel)args["Model"],
            };
        }

        public static TestCollectionModel CreateCollectionModel(Dictionary<string, object> args)
        {
            var collectionItems = new List<ICollectionItemModel>();
            var itemsCount = (int)args["ItemsCount"];
            for (int k = 0; k < itemsCount; k++)
            {
                collectionItems.Add(
                    CreateCollectionItemModel(k)
                );
            }

            return new TestCollectionModel
            {
                UserId = args["Id"].ToString(),
                Collection = new Dictionary<string, List<ICollectionItemModel>>
                {
                    {
                        CollectionName,
                        collectionItems
                    }
                }
            };
        }

        #endregion

        #region BLOB

        public static Stream GetStream(string content)
        {
            return new MemoryStream(Encoding.Default.GetBytes(content));
        }

        #endregion

        #region BLOB aggregator initialization

        public static byte[] CreateBlobResult()
        {
            var bytes = Encoding.Default.GetBytes(BlobFieldContent);
            return bytes;
        }

        public static List<BlobImageModel> CreateBlobAggregatorImages()
        {
            return new List<BlobImageModel>
            {
                new BlobImageModel
                {
                    FieldName = BlobFieldName,
                    Value = BlobFieldContent
                }
            };
        }

        #endregion

        #region Generic BLOB aggregator initialization

        public static CreateTestGenericBlobAggregatorModel CreateCreateTestGenericBlobAggregatorModel(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new CreateTestGenericBlobAggregatorModel
            {
                UserId = id.ToString(),
                TestProperty = $"{_propertyPrefix}{id}",
                Images = (List<BlobImageModel>)args["Images"]
            };
        }

        public static UpdateTestGenericBlobAggregatorModel CreateUpdateTestGenericBlobAggregatorModel(Dictionary<string, object> args)
        {
            var id = args["Id"];
            return new UpdateTestGenericBlobAggregatorModel
            {
                UserId = id.ToString(),
                TestProperty = $"{_propertyPrefix}{id}",
                Images = (List<BlobImageModel>)args["Images"]
            };
        }

        public static GenericBlobAggregatorSettings CreateGenericBlobAggregatorSettings()
        {
            return new GenericBlobAggregatorSettings
            {
                ContainerName = ContainerName,
                BlobFields = new string[]
                {
                    BlobFieldName
                }
            };
        }

        #endregion

        #region Collection BLOB aggregator initialization

        public static CreateTestCollectionBlobAggregatorModel CreateCreateTestCollectionBlobAggregatorModel(Dictionary<string, object> args)
        {
            return new CreateTestCollectionBlobAggregatorModel
            {
                UserId = args["Id"].ToString(),
                Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>()
            };
        }

        public static TestCollectionBlobAggregatorItemModel CreateTestCollectionBlobAggregatorItemModel(Dictionary<string, object> args)
        {
            args.TryGetValue("Id", out var index);
            index ??= 1;

            return new TestCollectionBlobAggregatorItemModel
            {
                Model = CreateCollectionItemModel(index),
                Images = CreateBlobAggregatorImages()
            };
        }

        public static UpdateTestCollectionBlobAggregatorModel CreateUpdateTestCollectionBlobAggregatorModel(Dictionary<string, object> args)
        {
            return new UpdateTestCollectionBlobAggregatorModel
            {
                UserId = args["Id"].ToString(),
                CollectionName = args["CollectionName"].ToString(),
                Action = (UpdateCollectionAction)args["Action"],
                Model = (ICollectionBlobAggregatorItemModel<ICollectionItemModel>)args["Model"],
            };
        }

        public static CollectionBlobAggregatorSettings CreateCollectionBlobAggregatorSettings()
        {
            return new CollectionBlobAggregatorSettings
            {
                ContainerName = ContainerName,
                CollectionsBlobFields = new Dictionary<string, string[]>
                {
                    {
                        CollectionName,
                        new string[]
                        {
                            BlobFieldName
                        }
                    }
                }
            };
        }

        #endregion
    }
}