using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models;

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
    public static string FileFieldName { get; } = "FileField";
    public static string FileFieldContent { get; } = "FileFieldContent";

    public static Dictionary<string, object> CreateGenericModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            }
        };
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
}