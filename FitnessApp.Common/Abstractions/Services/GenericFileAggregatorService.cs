using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Files;
using FitnessApp.Common.Helpers;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services;

public interface IGenericFileAggregatorService<
    TGenericFileAggregatorModel,
    TGenericModel,
    TCreateGenericFileAggregatorModel,
    TUpdateGenericFileAggregatorModel>
    where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
{
    Task<TGenericFileAggregatorModel> GetItemByUserId(string userId);
    Task<TGenericFileAggregatorModel[]> GetItemsByUserIds(string[] userIds);
    Task<PagedDataModel<TGenericFileAggregatorModel>> GetItems(GetPagedByIdsDataModel model);
    Task<TGenericFileAggregatorModel> CreateItem(TCreateGenericFileAggregatorModel model);
    Task<TGenericFileAggregatorModel> UpdateItem(TUpdateGenericFileAggregatorModel model);
    Task<string> DeleteItem(string userId);
}

public abstract class GenericFileAggregatorService<
    TGenericFileAggregatorModel,
    TGenericModel,
    TCreateGenericFileAggregatorModel,
    TCreateGenericModel,
    TUpdateGenericFileAggregatorModel,
    TUpdateGenericModel> :
    IGenericFileAggregatorService<
        TGenericFileAggregatorModel,
        TGenericModel,
        TCreateGenericFileAggregatorModel,
        TUpdateGenericFileAggregatorModel>
    where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    protected IGenericService<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel> GenericService
    { get; }
    protected IFilesService FilesService { get; }
    protected IMapper Mapper { get; }
    protected GenericFileAggregatorSettings GenericFileAggregatorSettings { get; }

    protected GenericFileAggregatorService(
        IGenericService<
            TGenericModel,
            TCreateGenericModel,
            TUpdateGenericModel> genericService,
        IFilesService filesService,
        IMapper mapper,
        GenericFileAggregatorSettings genericFileAggregatorSettings)
    {
        GenericService = genericService;
        FilesService = filesService;
        Mapper = mapper;
        GenericFileAggregatorSettings = genericFileAggregatorSettings;
    }

    public async Task<TGenericFileAggregatorModel> GetItemByUserId(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var dataModel = await GenericService.GetItemByUserId(userId);
        var result = await LoadAndComposeGenericFileAggregatorModel(dataModel);
        return result;
    }

    public async Task<TGenericFileAggregatorModel[]> GetItemsByUserIds(string[] userIds)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyIdsField(nameof(userIds), userIds);

        var dataModels = await GenericService.GetItemByUserIds(userIds);
        var result = await LoadAndComposeGenericFileAggregatorModels(dataModels);
        return result;
    }

    public async Task<PagedDataModel<TGenericFileAggregatorModel>> GetItems(GetPagedByIdsDataModel model)
    {
        var errors = new List<ValidationError>();
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyIdsField(nameof(model.UserIds), model.UserIds);
        errors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
        errors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
        errors.ThrowIfNotEmpty();

        var dataModels = await GenericService.GetItems(model);
        var result = (await LoadAndComposeGenericFileAggregatorModels(dataModels.Items)).ToPaged(model);
        return result;
    }

    public async Task<TGenericFileAggregatorModel> CreateItem(TCreateGenericFileAggregatorModel model)
    {
        var validationErrors = ValidateCreateGenericModel(model);
        validationErrors.ThrowIfNotEmpty();

        var createGenericModel = Mapper.Map<TCreateGenericModel>(model);
        var dataModel = await GenericService.CreateItem(createGenericModel);
        var result = await SaveAndComposeGenericFileAggregatorModel(dataModel, model.Images);
        return result;
    }

    public async Task<TGenericFileAggregatorModel> UpdateItem(TUpdateGenericFileAggregatorModel model)
    {
        var validationErrors = ValidateUpdateGenericModel(model);
        validationErrors.ThrowIfNotEmpty();
        ValidationHelper.ThrowExceptionIfNotValidFiles(model.Images);
        var updateGenericModel = Mapper.Map<TUpdateGenericModel>(model);
        var dataModel = await GenericService.UpdateItem(updateGenericModel);
        await DeleteItemFiles(model.UserId);
        var result = await SaveAndComposeGenericFileAggregatorModel(dataModel, model.Images);
        return result;
    }

    public async Task<string> DeleteItem(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var deleted = await GenericService.DeleteItem(userId);
        await DeleteItemFiles(userId);
        return deleted;
    }

    protected async Task<TGenericFileAggregatorModel> LoadAndComposeGenericFileAggregatorModel(TGenericModel dataModel)
    {
        var result = Activator.CreateInstance<TGenericFileAggregatorModel>();
        result.Model = dataModel;
        result.Images = [];
        foreach (var fileField in GenericFileAggregatorSettings.FileFields)
        {
            var fileContent = await FilesService.DownloadFile(GenericFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, dataModel.UserId));
            if (fileContent != null)
            {
                result.Images.Add(new FileImageModel
                {
                    FieldName = fileField,
                    Value = Encoding.Default.GetString(fileContent)
                });
            }
        }

        return result;
    }

    protected async Task<TGenericFileAggregatorModel[]> LoadAndComposeGenericFileAggregatorModels(TGenericModel[] dataModels)
    {
        var loadAndComposeGenericFileAggregatorModelTasks = dataModels.Select(LoadAndComposeGenericFileAggregatorModel);
        return [.. await Task.WhenAll(loadAndComposeGenericFileAggregatorModelTasks)];
    }

    protected virtual List<ValidationError> ValidateCreateGenericModel(TCreateGenericFileAggregatorModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        ValidationHelper.ThrowExceptionIfNotValidFiles(model.Images);
        return errors;
    }

    protected virtual List<ValidationError> ValidateUpdateGenericModel(TUpdateGenericFileAggregatorModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        return errors;
    }

    private async Task<TGenericFileAggregatorModel> SaveAndComposeGenericFileAggregatorModel(TGenericModel dataModel, FileImageModel[] fileFields)
    {
        var result = Activator.CreateInstance<TGenericFileAggregatorModel>();
        result.Model = dataModel;
        result.Images = [];
        foreach (var fileField in fileFields)
        {
            if (fileField.Value != null)
            {
                var fileContent = Encoding.Default.GetBytes(fileField.Value);
                await FilesService.UploadFile(
                    GenericFileAggregatorSettings.ContainerName,
                    FilesService.CreateFileName(fileField.FieldName, result.Model.UserId),
                    new MemoryStream(fileContent));
                result.Images.Add(fileField);
            }
        }

        return result;
    }

    private async Task DeleteItemFiles(string userId)
    {
        foreach (var fileField in GenericFileAggregatorSettings.FileFields)
        {
            await FilesService.DeleteFile(GenericFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, userId));
        }
    }
}