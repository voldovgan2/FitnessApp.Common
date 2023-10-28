using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Abstractions.Services.Collection;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Files;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.CollectionBlobAggregator
{
    public abstract class CollectionBlobAggregatorService<TCollectionBlobAggregatorModel, TCollectionBlobAggregatorItemModel, TCollectionModel, TCollectionItemModel, TCreateCollectionBlobAggregatorModel, TCreateCollectionModel, TUpdateCollectionBlobAggregatorModel, TUpdateCollectionModel>
        : ICollectionBlobAggregatorService<TCollectionBlobAggregatorModel, TCollectionBlobAggregatorItemModel, TCollectionItemModel, TCreateCollectionBlobAggregatorModel, TUpdateCollectionBlobAggregatorModel>
        where TCollectionBlobAggregatorModel : ICollectionBlobAggregatorModel
        where TCollectionBlobAggregatorItemModel : ICollectionBlobAggregatorItemModel<ICollectionItemModel>
        where TCollectionModel : ICollectionModel
        where TCollectionItemModel : ICollectionItemModel
        where TCreateCollectionBlobAggregatorModel : ICreateCollectionBlobAggregatorModel
        where TCreateCollectionModel : ICreateCollectionModel
        where TUpdateCollectionBlobAggregatorModel : IUpdateCollectionBlobAggregatorModel
        where TUpdateCollectionModel : IUpdateCollectionModel
    {
        private readonly ICollectionService<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel> _collectionService;
        protected readonly IFilesService _blobService;
        protected readonly IMapper _mapper;
        private readonly CollectionBlobAggregatorSettings _collectionBlobAggregatorSettings;

        protected CollectionBlobAggregatorService(
            ICollectionService<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel> collectionService,
            IFilesService blobService,
            IMapper mapper,
            CollectionBlobAggregatorSettings collectionBlobAggregatorSettings
        )
        {
            _collectionService = collectionService;
            _blobService = blobService;
            _mapper = mapper;
            _collectionBlobAggregatorSettings = collectionBlobAggregatorSettings;
        }

        public virtual async Task<TCollectionBlobAggregatorModel> GetItemByUserId(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var dataModel = await _collectionService.GetItemByUserId(userId);
            var result = await LoadAndComposeGenericBlobAggregatorModel(dataModel);
            return result;
        }

        public virtual async Task<PagedDataModel<TCollectionBlobAggregatorItemModel>> GetFilteredCollectionItems(GetFilteredCollectionItemsModel<TCollectionItemModel> model)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
            validationErrors.ThrowIfNotEmpty();

            var allItems = await _collectionService.GetCollectionByUserId(model.UserId, model.CollectionName);

            // Should we filter it here or in collection service
#pragma warning disable S125 // Sections of code should not be commented out

            // var itemsBySearch = await _searchService.Search(search);
            allItems = allItems.Where(model.Predicate);
#pragma warning restore S125 // Sections of code should not be commented out
            var pagedItems = allItems.ToPaged(model);
            var result = new PagedDataModel<TCollectionBlobAggregatorItemModel>
            {
                TotalCount = pagedItems.TotalCount,
                Page = pagedItems.Page,
                Items = await LoadAndComposeCollectionBlobAggregatorItemModel(model.CollectionName, pagedItems.Items)
            };
            return result;
        }

        public virtual async Task<string> CreateItem(TCreateCollectionBlobAggregatorModel model)
        {
            var validationErrors = ValidateCreateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var createCollectionModel = _mapper.Map<TCreateCollectionModel>(model);
            var result = await _collectionService.CreateItem(createCollectionModel);
            return result;
        }

        public virtual async Task<TCollectionBlobAggregatorItemModel> UpdateItem(TUpdateCollectionBlobAggregatorModel model)
        {
            var validationErrors = ValidateUpdateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var updateGenericModel = _mapper.Map<TUpdateCollectionModel>(model);
            var dataModel = await _collectionService.UpdateItem(updateGenericModel);
            var collectionBlobFields = _collectionBlobAggregatorSettings.CollectionsBlobFields[model.CollectionName];
            await DeleteItemBlobs(model.Model.Model.Id, collectionBlobFields);
            var blobFields = model.Model.Images.Where(f => collectionBlobFields.Contains(f.FieldName));
            var result = await SaveAndComposeGenericBlobAggregatorModel(dataModel, blobFields);
            return result;
        }

        public virtual async Task<string> DeleteItem(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var deleted = await _collectionService.DeleteItem(userId);
            await DeleteItemBlobs(deleted);
            return deleted.UserId;
        }

        protected virtual IEnumerable<ValidationError> ValidateCreateCollectionModel(TCreateCollectionBlobAggregatorModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            return errors;
        }

        protected virtual IEnumerable<ValidationError> ValidateUpdateCollectionModel(TUpdateCollectionBlobAggregatorModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
            errors.AddRange(ValidateCollectionItemModel(model));
            return errors;
        }

        protected virtual IEnumerable<ValidationError> ValidateCollectionItemModel(TUpdateCollectionBlobAggregatorModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.Model.Model.Id), model.Model.Model?.Id));
            return errors;
        }

        private async Task<TCollectionBlobAggregatorItemModel> SaveAndComposeGenericBlobAggregatorModel(TCollectionItemModel dataModel, IEnumerable<BlobImageModel> blobFields)
        {
            var result = Activator.CreateInstance<TCollectionBlobAggregatorItemModel>();
            result.Model = dataModel;
            result.Images = new List<BlobImageModel>();

            foreach (var blobField in blobFields)
            {
                if (blobField.Value != null)
                {
                    var blobContent = Encoding.Default.GetBytes(blobField.Value);
                    await _blobService.UploadFile(_collectionBlobAggregatorSettings.ContainerName, FilesService.CreateBlobName(blobField.FieldName, result.Model.Id), new MemoryStream(blobContent));
                    result.Images.Add(blobField);
                }
            }

            return result;
        }

        private async Task<TCollectionBlobAggregatorModel> LoadAndComposeGenericBlobAggregatorModel(TCollectionModel model)
        {
            var result = Activator.CreateInstance<TCollectionBlobAggregatorModel>();
            result.UserId = model.UserId;
            result.Collection = new Dictionary<string, List<ICollectionBlobAggregatorItemModel<ICollectionItemModel>>>();
            foreach (var kvp in model.Collection)
            {
                var collectionItems = _mapper.Map<IEnumerable<TCollectionItemModel>>(kvp.Value);
                var collectionBlobAggregatorItems = await LoadAndComposeCollectionBlobAggregatorItemModel(kvp.Key, collectionItems);
                var collection = _mapper.Map<List<ICollectionBlobAggregatorItemModel<ICollectionItemModel>>>(collectionBlobAggregatorItems);
                result.Collection.Add(kvp.Key, collection);
            }

            return result;
        }

        private async Task<List<TCollectionBlobAggregatorItemModel>> LoadAndComposeCollectionBlobAggregatorItemModel(string collectionName, IEnumerable<TCollectionItemModel> collection)
        {
            var result = new List<TCollectionBlobAggregatorItemModel>();
            foreach (var collectionItemModel in collection)
            {
                var collectionBlobAggregatorItemModel = Activator.CreateInstance<TCollectionBlobAggregatorItemModel>();
                collectionBlobAggregatorItemModel.Model = collectionItemModel;
                collectionBlobAggregatorItemModel.Images = new List<BlobImageModel>();
                var collectionBlobFields = _collectionBlobAggregatorSettings.CollectionsBlobFields[collectionName];
                foreach (var blobField in collectionBlobFields)
                {
                    var blobContent = await _blobService.DownloadFile(_collectionBlobAggregatorSettings.ContainerName, FilesService.CreateBlobName(blobField, collectionItemModel.Id));
                    if (blobContent != null)
                    {
                        collectionBlobAggregatorItemModel.Images.Add(new BlobImageModel
                        {
                            FieldName = blobField,
                            Value = Encoding.Default.GetString(blobContent)
                        });
                    }
                }

                result.Add(collectionBlobAggregatorItemModel);
            }

            return result;
        }

        private async Task DeleteItemBlobs(TCollectionModel model)
        {
            foreach (var kvp in model.Collection)
            {
                var blobFields = _collectionBlobAggregatorSettings.CollectionsBlobFields[kvp.Key];
                foreach (var item in kvp.Value)
                {
                    await DeleteItemBlobs(item.Id, blobFields);
                }
            }
        }

        private async Task DeleteItemBlobs(string itemId, string[] blobFields)
        {
            foreach (var blobField in blobFields)
            {
                await _blobService.DeleteFile(_collectionBlobAggregatorSettings.ContainerName, FilesService.CreateBlobName(blobField, itemId));
            }
        }
    }
}