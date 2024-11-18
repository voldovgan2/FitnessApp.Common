using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Abstractions.Models;

public interface IModel
{
    string Id { get; set; }
}

public interface IWithUserIdModel
{
    string UserId { get; set; }
}

public interface IGenericModel : IModel, IWithUserIdModel;

public interface ICreateModel;

public interface IUpdateModel;

public interface ICreateGenericModel : ICreateModel, IWithUserIdModel;

public interface IUpdateGenericModel : IUpdateModel, IModel, IWithUserIdModel;

public interface IGenericFileAggregatorModel<TGenericModel> : IModel
    where TGenericModel : IGenericModel
{
    public TGenericModel Model { get; set; }
    public List<FileImageModel> Images { get; set; }
}

public interface ICreateGenericFileAggregatorModel : ICreateModel, IWithUserIdModel
{
    FileImageModel[] Images { get; set; }
}

public interface IUpdateGenericFileAggregatorModel : IUpdateModel, IModel, IWithUserIdModel
{
    FileImageModel[] Images { get; set; }
}

[ExcludeFromCodeCoverage]
public class FileImageModel
{
    public string FieldName { get; set; }
    public string Value { get; set; }
}