using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Services;
using FitnessApp.Comon.Tests.Shared.Abstraction.Db;
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
    public static string[] Ids { get; } = [Id];
    public static string CollectionName { get; } = "Collection";
    public static string ContainerName { get; } = "ContainerName";
    public static string FileFieldName { get; } = "FileField";
    public static string FileFieldContent { get; } = "FileFieldContent";

    private static string _propertyPrefix = "TestProperty";

    #region Abstract

    public static T[] GetAll<T>(Func<Dictionary<string, object>, T> createElementFactory, Dictionary<string, object> args)
        where T : class
    {
        var result = new T[4];
        args.Add("Id", 0);

        for (int k = 0; k < 4; k++)
        {
            args["Id"] = k + 1;
            result[k] = createElementFactory(args);
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

    public static FileImageModel[] CreateFileAggregatorImages()
    {
        return [
            new FileImageModel
            {
                FieldName = FileFieldName,
                Value = FileFieldContent
            },
        ];
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
            Images = (FileImageModel[])args["Images"]
        };
    }

    public static UpdateTestGenericFileAggregatorModel CreateUpdateTestGenericFileAggregatorModel(Dictionary<string, object> args)
    {
        var id = args["Id"];
        return new UpdateTestGenericFileAggregatorModel
        {
            UserId = id?.ToString(),
            TestProperty = $"{_propertyPrefix}{id}",
            Images = (FileImageModel[])args["Images"]
        };
    }

    public static GenericFileAggregatorSettings CreateGenericFileAggregatorSettings()
    {
        return new GenericFileAggregatorSettings
        {
            ContainerName = ContainerName,
            FileFields =
            [
                FileFieldName
            ]
        };
    }

    #endregion
}