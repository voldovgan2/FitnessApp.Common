using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Files;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.CollectionFileAggregator;

public abstract class CollectionFileAggregatorService<
    TCollectionFileAggregatorModel,
    TCollectionFileAggregatorItemModel,
    TCollectionModel, TCollectionItemModel,
    TCreateCollectionFileAggregatorModel,
    TCreateCollectionModel,
    TUpdateCollectionFileAggregatorModel,
    TUpdateCollectionModel>(
        ICollectionService<
            TCollectionModel,
            TCollectionItemModel,
            TCreateCollectionModel,
            TUpdateCollectionModel> collectionService,
        IFilesService filesService,
        IMapper mapper,
        CollectionFileAggregatorSettings collectionFileAggregatorSettings
    ) : ICollectionFileAggregatorService<
        TCollectionFileAggregatorModel,
        TCollectionFileAggregatorItemModel,
        TCreateCollectionFileAggregatorModel,
        TUpdateCollectionFileAggregatorModel>
    where TCollectionFileAggregatorModel : ICollectionFileAggregatorModel
    where TCollectionFileAggregatorItemModel : ICollectionFileAggregatorItemModel<ICollectionItemModel>
    where TCollectionModel : ICollectionModel
    where TCollectionItemModel : ICollectionItemModel
    where TCreateCollectionFileAggregatorModel : ICreateCollectionFileAggregatorModel
    where TCreateCollectionModel : ICreateCollectionModel
    where TUpdateCollectionFileAggregatorModel : IUpdateCollectionFileAggregatorModel
    where TUpdateCollectionModel : IUpdateCollectionModel
{
    public async Task<TCollectionFileAggregatorModel> GetItemByUserId(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var dataModel = await collectionService.GetItemByUserId(userId);
        var result = await LoadAndComposeGenericFileAggregatorModel(dataModel);
        return result;
    }

    public async Task<IEnumerable<TCollectionFileAggregatorItemModel>> GetCollectionByUserId(string userId, string collectionName)
    {
        List<ValidationError> errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(userId), userId));
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(collectionName), collectionName));
        errors.ThrowIfNotEmpty();

        var allItems = await collectionService.GetCollectionByUserId(userId, collectionName);
        return await LoadAndComposeCollectionFileAggregatorItemModel(collectionName, allItems);
    }

    public async Task<PagedDataModel<TCollectionFileAggregatorItemModel>> GetFilteredCollectionItems(GetFilteredCollectionItemsModel model)
    {
        List<ValidationError> errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
        errors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
        errors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
        errors.ThrowIfNotEmpty();

        var pagedItems = await collectionService.GetFilteredCollectionItems(model);
        var result = new PagedDataModel<TCollectionFileAggregatorItemModel>
        {
            TotalCount = pagedItems.TotalCount,
            Page = pagedItems.Page,
            Items = await LoadAndComposeCollectionFileAggregatorItemModel(model.CollectionName, pagedItems.Items)
        };
        return result;
    }

    public async Task<string> CreateItem(TCreateCollectionFileAggregatorModel model)
    {
        var validationErrors = ValidateCreateCollectionModel(model);
        validationErrors.ThrowIfNotEmpty();

        var createCollectionModel = mapper.Map<TCreateCollectionModel>(model);
        var result = await collectionService.CreateItem(createCollectionModel);
        return result;
    }

    public async Task<TCollectionFileAggregatorItemModel> UpdateItem(TUpdateCollectionFileAggregatorModel model)
    {
        var validationErrors = ValidateUpdateCollectionModel(model);
        validationErrors.ThrowIfNotEmpty();

        var updateGenericModel = mapper.Map<TUpdateCollectionModel>(model);
        var dataModel = await collectionService.UpdateItem(updateGenericModel);
        var collectionFileFields = collectionFileAggregatorSettings.CollectionsFileFields[model.CollectionName];
        await DeleteItemFiles(model.Model.Model.Id, collectionFileFields);
        var fileFields = model.Model.Images.Where(f => collectionFileFields.Contains(f.FieldName));
        var result = await SaveAndComposeGenericFileAggregatorModel(dataModel, fileFields);
        return result;
    }

    public async Task<string> DeleteItem(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var deleted = await collectionService.DeleteItem(userId);
        await DeleteItemFiles(deleted);
        return deleted.UserId;
    }

    protected virtual IEnumerable<ValidationError> ValidateCreateCollectionModel(TCreateCollectionFileAggregatorModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        return errors;
    }

    protected virtual IEnumerable<ValidationError> ValidateUpdateCollectionModel(TUpdateCollectionFileAggregatorModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
        errors.AddRange(ValidateCollectionItemModel(model));
        return errors;
    }

    protected virtual IEnumerable<ValidationError> ValidateCollectionItemModel(TUpdateCollectionFileAggregatorModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.Model.Model.Id), model.Model.Model?.Id));
        return errors;
    }

    private async Task<TCollectionFileAggregatorItemModel> SaveAndComposeGenericFileAggregatorModel(TCollectionItemModel dataModel, IEnumerable<FileImageModel> fileFields)
    {
        var result = Activator.CreateInstance<TCollectionFileAggregatorItemModel>();
        result.Model = dataModel;
        result.Images = new List<FileImageModel>();

        foreach (var fileField in fileFields)
        {
            if (fileField.Value != null)
            {
                var fileContent = Encoding.Default.GetBytes(fileField.Value);
                await filesService.UploadFile(
                    collectionFileAggregatorSettings.ContainerName,
                    FilesService.CreateFileName(fileField.FieldName, result.Model.Id),
                    new MemoryStream(fileContent));
                result.Images.Add(fileField);
            }
        }

        return result;
    }

    private async Task<TCollectionFileAggregatorModel> LoadAndComposeGenericFileAggregatorModel(TCollectionModel model)
    {
        var result = Activator.CreateInstance<TCollectionFileAggregatorModel>();
        result.UserId = model.UserId;
        result.Collection = new Dictionary<string, List<ICollectionFileAggregatorItemModel<ICollectionItemModel>>>();
        foreach (var kvp in model.Collection)
        {
            var collectionItems = mapper.Map<IEnumerable<TCollectionItemModel>>(kvp.Value);
            var collectionFileAggregatorItems = await LoadAndComposeCollectionFileAggregatorItemModel(kvp.Key, collectionItems);
            var collection = mapper.Map<List<ICollectionFileAggregatorItemModel<ICollectionItemModel>>>(collectionFileAggregatorItems);
            result.Collection.Add(kvp.Key, collection);
        }

        return result;
    }

    private async Task<List<TCollectionFileAggregatorItemModel>> LoadAndComposeCollectionFileAggregatorItemModel(string collectionName, IEnumerable<TCollectionItemModel> collection)
    {
        var result = new List<TCollectionFileAggregatorItemModel>();
        foreach (var collectionItemModel in collection)
        {
            var collectionFileAggregatorItemModel = Activator.CreateInstance<TCollectionFileAggregatorItemModel>();
            collectionFileAggregatorItemModel.Model = collectionItemModel;
            collectionFileAggregatorItemModel.Images = new List<FileImageModel>();
            var collectionFileFields = collectionFileAggregatorSettings.CollectionsFileFields[collectionName];
            foreach (var fileField in collectionFileFields)
            {
                var fileContent = await filesService.DownloadFile(collectionFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, collectionItemModel.Id));
                if (fileContent != null)
                {
                    collectionFileAggregatorItemModel.Images.Add(new FileImageModel
                    {
                        FieldName = fileField,
                        Value = Encoding.Default.GetString(fileContent)
                    });
                }
            }

            result.Add(collectionFileAggregatorItemModel);
        }

        return result;
    }

    private async Task DeleteItemFiles(TCollectionModel model)
    {
        foreach (var kvp in model.Collection)
        {
            var fileFields = collectionFileAggregatorSettings.CollectionsFileFields[kvp.Key];
            foreach (var item in kvp.Value)
            {
                await DeleteItemFiles(item.Id, fileFields);
            }
        }
    }

    private async Task DeleteItemFiles(string itemId, string[] fileFields)
    {
        foreach (var fileField in fileFields)
        {
            await filesService.DeleteFile(collectionFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, itemId));
        }
    }
}