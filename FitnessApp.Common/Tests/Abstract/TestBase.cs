using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Paged;

namespace FitnessApp.Common.Tests.Abstract;

public abstract class TestBase<
    TModel,
    TCreateModel,
    TUpdateModel>
    where TModel : IModel
    where TCreateModel : ICreateModel
    where TUpdateModel : IUpdateModel
{
    public string Id { get; } = "1";
    public string[] Ids => [Id];
    public string FileFieldName { get; } = "FileField";
    public string FileFieldContent { get; } = "FileFieldContent";

    protected Dictionary<string, object> CreateGenericModelParameters(string id)
    {
        return new()
        {
            {
                "Id", id
            }
        };
    }

    protected FileImageModel[] CreateFileAggregatorImages()
    {
        return [
            new FileImageModel
            {
                FieldName = FileFieldName,
                Value = FileFieldContent
            },
        ];
    }

    protected abstract GetPagedByIdsDataModel CreateGetPagedByIdsDataModel(
        string[] userIds,
        int page,
        int PageSize);

    protected abstract TCreateModel CreateCreateModel(Dictionary<string, object> args);

    protected abstract TUpdateModel CreateUpdateModel(Dictionary<string, object> args);

    protected abstract void AssertExtractedItem(TModel model);

    protected abstract void AssertCreatedItem(TModel model);

    protected abstract void AssertUpdatedItem(TModel model);

    protected virtual void AssertCollection(TModel[] models)
    {
        foreach (var model in models)
        {
            AssertExtractedItem(model);
        }
    }
}