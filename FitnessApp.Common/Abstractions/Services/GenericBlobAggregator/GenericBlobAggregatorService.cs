using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Blob;

namespace FitnessApp.Common.Abstractions.Services.GenericBlobAggregator
{
    public abstract class GenericBlobAggregatorService<TGenericEntity, TGenericBlobAggregatorModel, TGenericModel, TCreateGenericBlobAggregatorModel, TCreateGenericModel, TUpdateGenericBlobAggregatorModel, TUpdateGenericModel>
        : IGenericBlobAggregatorService<TGenericEntity, TGenericBlobAggregatorModel, TGenericModel, TCreateGenericBlobAggregatorModel, TUpdateGenericBlobAggregatorModel>
        where TGenericEntity : IGenericEntity
        where TGenericBlobAggregatorModel : IGenericBlobAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericBlobAggregatorModel : ICreateGenericBlobAggregatorModel
        where TCreateGenericModel : ICreateGenericModel
        where TUpdateGenericBlobAggregatorModel : IUpdateGenericBlobAggregatorModel
        where TUpdateGenericModel : IUpdateGenericModel
    {
        protected readonly IGenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> _genericService;
        protected readonly IBlobService _blobService;
        protected readonly IMapper _mapper;
        protected readonly GenericBlobAggregatorSettings _genericBlobAggregatorSettings;

        protected GenericBlobAggregatorService(
            IGenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> genericService,
            IBlobService blobService,
            IMapper mapper,
            GenericBlobAggregatorSettings genericBlobAggregatorSettings
        )
        {
            _genericService = genericService;
            _blobService = blobService;
            _mapper = mapper;
            _genericBlobAggregatorSettings = genericBlobAggregatorSettings;
        }

        public async Task<TGenericBlobAggregatorModel> CreateItem(TCreateGenericBlobAggregatorModel model)
        {
            ValidationHelper.ThrowExceptionIfNotValidBlobs(model.Images);

            var createGenericModel = _mapper.Map<TCreateGenericModel>(model);
            var dataModel = await _genericService.CreateItem(createGenericModel);
            var result = await SaveAndComposeGenericBlobAggregatorModel(dataModel, model.Images);
            return result;
        }

        public async Task<TGenericBlobAggregatorModel> UpdateItem(TUpdateGenericBlobAggregatorModel model)
        {
            ValidationHelper.ThrowExceptionIfNotValidBlobs(model.Images);

            var updateGenericModel = _mapper.Map<TUpdateGenericModel>(model);
            var dataModel = await _genericService.UpdateItem(updateGenericModel);
            await DeleteItemBlobs(model.UserId);
            var result = await SaveAndComposeGenericBlobAggregatorModel(dataModel, model.Images);
            return result;
        }

        public async Task<string> DeleteItem(string userId)
        {
            var deleted = await _genericService.DeleteItem(userId);
            await DeleteItemBlobs(userId);
            return deleted;
        }

        public async Task<TGenericBlobAggregatorModel> GetItem(string userId)
        {
            var dataModel = await _genericService.GetItemByUserId(userId);
            var result = await LoadAndComposeGenericBlobAggregatorModel(dataModel);
            return result;
        }

        public async Task<IEnumerable<TGenericBlobAggregatorModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate)
        {
            var dataModels = await _genericService.GetItems(search, predicate);
            var result = await LoadAndComposeGenericBlobAggregatorModels(dataModels);
            return result;
        }

        public async Task<IEnumerable<TGenericBlobAggregatorModel>> GetItems(string[] ids)
        {
            var dataModels = await _genericService.GetItems(ids);
            var result = await LoadAndComposeGenericBlobAggregatorModels(dataModels);
            return result;
        }

        private async Task<TGenericBlobAggregatorModel> SaveAndComposeGenericBlobAggregatorModel(TGenericModel dataModel, IEnumerable<BlobImageModel> blobFields)
        {
            var result = Activator.CreateInstance<TGenericBlobAggregatorModel>();
            result.Model = dataModel;
            result.Images = new List<BlobImageModel>();

            foreach (var blobField in blobFields)
            {
                if (blobField.Value != null)
                {
                    var blobContent = Encoding.Default.GetBytes(blobField.Value);
                    await _blobService.UploadFile(_genericBlobAggregatorSettings.ContainerName, BlobService.CreateBlobName(blobField.FieldName, result.Model.UserId), new MemoryStream(blobContent));
                    result.Images.Add(blobField);
                }
            }

            return result;
        }

        private async Task<TGenericBlobAggregatorModel> LoadAndComposeGenericBlobAggregatorModel(TGenericModel dataModel)
        {
            var result = Activator.CreateInstance<TGenericBlobAggregatorModel>();
            result.Model = dataModel;
            result.Images = new List<BlobImageModel>();

            foreach (var blobField in _genericBlobAggregatorSettings.BlobFields)
            {
                var blobContent = await _blobService.DownloadFile(_genericBlobAggregatorSettings.ContainerName, BlobService.CreateBlobName(blobField, dataModel.UserId));
                if (blobContent != null)
                {
                    result.Images.Add(new BlobImageModel
                    {
                        FieldName = blobField,
                        Value = Encoding.Default.GetString(blobContent)
                    });
                }
            }

            return result;
        }

        private async Task<IEnumerable<TGenericBlobAggregatorModel>> LoadAndComposeGenericBlobAggregatorModels(IEnumerable<TGenericModel> dataModels)
        {
            var result = new List<TGenericBlobAggregatorModel>();
            foreach (var dataModel in dataModels)
            {
                result.Add(await LoadAndComposeGenericBlobAggregatorModel(dataModel));
            }

            return result;
        }

        private async Task DeleteItemBlobs(string userId)
        {
            foreach (var blobField in _genericBlobAggregatorSettings.BlobFields)
            {
                await _blobService.DeleteFile(_genericBlobAggregatorSettings.ContainerName, BlobService.CreateBlobName(blobField, userId));
            }
        }
    }
}