using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using FitnessApp.Common.Abstractions.Db.Enums.Collection;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.CollectionFileAggregator;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;
using FitnessApp.Comon.Tests.Shared.Abstraction.Models.GenericFileAggregator;

namespace FitnessApp.Comon.Tests.Shared;

[ExcludeFromCodeCoverageAttribute]
public static class TestData
{
    public static string EntityIdToGet { get; } = "EntityIdToGet";
    public static string EntityIdToCreate { get; } = "EntityIdToCreate";
    public static string EntityIdToUpdate { get; } = "EntityIdToUpdate";
    public static string EntityIdToDelete { get; } = "EntityIdToDelete";

    public static string FileToDownload { get; } = "FileToDownload";
    public static string FileToUpload { get; } = "FileToUpload";
    public static string FileToDelete { get; } = "FileToDelete";

    public static string Id { get; } = "1";
    public static string[] Ids { get; } = new string[] { Id };
    public static string CollectionName { get; } = "Collection";
    public static string ContainerName { get; } = "ContainerName";
    public static string FileFieldName { get; } = "FileField";
    public static string FileFieldContent { get; } = "FileFieldContent";

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
            UserId = id?.ToString(),
            TestProperty1 = $"{_propertyPrefix}{id}"
        };
    }

    public static CreateTestGenericModel CreateCreateTestGenericModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new CreateTestGenericModel
        {
            UserId = id?.ToString(),
            TestProperty1 = $"{_propertyPrefix}{id}"
        };
    }

    public static UpdateTestGenericModel CreateUpdateTestGenericModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new UpdateTestGenericModel
        {
            UserId = id?.ToString(),
            TestProperty1 = $"{_propertyPrefix}{id}"
        };
    }

    public static TestGenericModel CreateGenericModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new TestGenericModel
        {
            UserId = id?.ToString(),
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
            UserId = args["Id"]?.ToString(),
            Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>()
        };
    }

    public static UpdateTestCollectionModel CreateUpdateTestCollectionModel(Dictionary<string, object> args)
    {
        return new UpdateTestCollectionModel
        {
            UserId = args["Id"]?.ToString(),
            CollectionName = args["CollectionName"]?.ToString(),
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

    #region File

    public static Stream GetStream(string content)
    {
        return new MemoryStream(Encoding.Default.GetBytes(content));
    }

    #endregion

    #region File aggregator initialization

    public static byte[] CreateFileResult()
    {
        var bytes = Encoding.Default.GetBytes(FileFieldContent);
        return bytes;
    }

    public static List<FileImageModel> CreateFileAggregatorImages()
    {
        return new List<FileImageModel>
        {
            new FileImageModel
            {
                FieldName = FileFieldName,
                Value = FileFieldContent
            }
        };
    }

    #endregion

    #region Generic File aggregator initialization

    public static CreateTestGenericFileAggregatorModel CreateCreateTestGenericFileAggregatorModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new CreateTestGenericFileAggregatorModel
        {
            UserId = id?.ToString(),
            TestProperty = $"{_propertyPrefix}{id}",
            Images = (List<FileImageModel>)args["Images"]
        };
    }

    public static UpdateTestGenericFileAggregatorModel CreateUpdateTestGenericFileAggregatorModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new UpdateTestGenericFileAggregatorModel
        {
            UserId = id?.ToString(),
            TestProperty = $"{_propertyPrefix}{id}",
            Images = (List<FileImageModel>)args["Images"]
        };
    }

    public static GenericFileAggregatorSettings CreateGenericFileAggregatorSettings()
    {
        return new GenericFileAggregatorSettings
        {
            ContainerName = ContainerName,
            FileFields = new string[]
            {
                FileFieldName
            }
        };
    }

    #endregion

    #region Collection File aggregator initialization

    public static CreateTestCollectionFileAggregatorModel CreateCreateTestCollectionFileAggregatorModel(Dictionary<string, object> args)
    {
        return new CreateTestCollectionFileAggregatorModel
        {
            UserId = args["Id"]?.ToString(),
            Collection = new Dictionary<string, IEnumerable<ICollectionItemModel>>()
        };
    }

    public static TestCollectionFileAggregatorItemModel CreateTestCollectionFileAggregatorItemModel(Dictionary<string, object> args)
    {
        args.TryGetValue("Id", out var index);
        index ??= 1;

        return new TestCollectionFileAggregatorItemModel
        {
            Model = CreateCollectionItemModel(index),
            Images = CreateFileAggregatorImages()
        };
    }

    public static UpdateTestCollectionFileAggregatorModel CreateUpdateTestCollectionFileAggregatorModel(Dictionary<string, object> args)
    {
        return new UpdateTestCollectionFileAggregatorModel
        {
            UserId = args["Id"]?.ToString(),
            CollectionName = args["CollectionName"]?.ToString(),
            Action = (UpdateCollectionAction)args["Action"],
            Model = (ICollectionFileAggregatorItemModel<ICollectionItemModel>)args["Model"],
        };
    }

    public static CollectionFileAggregatorSettings CreateCollectionFileAggregatorSettings()
    {
        return new CollectionFileAggregatorSettings
        {
            ContainerName = ContainerName,
            CollectionsFileFields = new Dictionary<string, string[]>
            {
                {
                    CollectionName,
                    new string[]
                    {
                        FileFieldName
                    }
                }
            }
        };
    }

    #endregion
}