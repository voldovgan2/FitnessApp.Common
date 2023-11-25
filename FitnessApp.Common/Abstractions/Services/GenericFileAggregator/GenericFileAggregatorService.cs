using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.FileImage;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;
using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Abstractions.Services.Generic;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Files;

namespace FitnessApp.Common.Abstractions.Services.GenericFileAggregator
{
    public abstract class GenericFileAggregatorService<TGenericEntity, TGenericFileAggregatorModel, TGenericModel, TCreateGenericFileAggregatorModel, TCreateGenericModel, TUpdateGenericFileAggregatorModel, TUpdateGenericModel>
        : IGenericFileAggregatorService<TGenericEntity, TGenericFileAggregatorModel, TGenericModel, TCreateGenericFileAggregatorModel, TUpdateGenericFileAggregatorModel>
        where TGenericEntity : IGenericEntity
        where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
        where TCreateGenericModel : ICreateGenericModel
        where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
        where TUpdateGenericModel : IUpdateGenericModel
    {
        protected readonly IGenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> _genericService;
        protected readonly IFilesService _filesService;
        protected readonly IMapper _mapper;
        protected readonly GenericFileAggregatorSettings _genericFileAggregatorSettings;

        protected GenericFileAggregatorService(
            IGenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> genericService,
            IFilesService filesService,
            IMapper mapper,
            GenericFileAggregatorSettings genericFileAggregatorSettings
        )
        {
            _genericService = genericService;
            _filesService = filesService;
            _mapper = mapper;
            _genericFileAggregatorSettings = genericFileAggregatorSettings;
        }

        public async Task<TGenericFileAggregatorModel> CreateItem(TCreateGenericFileAggregatorModel model)
        {
            ValidationHelper.ThrowExceptionIfNotValidFiles(model.Images);

            var createGenericModel = _mapper.Map<TCreateGenericModel>(model);
            var dataModel = await _genericService.CreateItem(createGenericModel);
            var result = await SaveAndComposeGenericFileAggregatorModel(dataModel, model.Images);
            return result;
        }

        public async Task<TGenericFileAggregatorModel> UpdateItem(TUpdateGenericFileAggregatorModel model)
        {
            ValidationHelper.ThrowExceptionIfNotValidFiles(model.Images);

            var updateGenericModel = _mapper.Map<TUpdateGenericModel>(model);
            var dataModel = await _genericService.UpdateItem(updateGenericModel);
            await DeleteItemFiles(model.UserId);
            var result = await SaveAndComposeGenericFileAggregatorModel(dataModel, model.Images);
            return result;
        }

        public async Task<string> DeleteItem(string userId)
        {
            var deleted = await _genericService.DeleteItem(userId);
            await DeleteItemFiles(userId);
            return deleted;
        }

        public async Task<TGenericFileAggregatorModel> GetItem(string userId)
        {
            var dataModel = await _genericService.GetItemByUserId(userId);
            var result = await LoadAndComposeGenericFileAggregatorModel(dataModel);
            return result;
        }

        public async Task<IEnumerable<TGenericFileAggregatorModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate)
        {
            var dataModels = await _genericService.GetItems(search, predicate);
            var result = await LoadAndComposeGenericFileAggregatorModels(dataModels);
            return result;
        }

        public async Task<IEnumerable<TGenericFileAggregatorModel>> GetItems(string[] ids)
        {
            var dataModels = await _genericService.GetItems(ids);
            var result = await LoadAndComposeGenericFileAggregatorModels(dataModels);
            return result;
        }

        private async Task<TGenericFileAggregatorModel> SaveAndComposeGenericFileAggregatorModel(
            TGenericModel dataModel,
            IEnumerable<FileImageModel> fileFields)
        {
            var result = Activator.CreateInstance<TGenericFileAggregatorModel>();
            result.Model = dataModel;
            result.Images = new List<FileImageModel>();

            foreach (var fileField in fileFields)
            {
                if (fileField.Value != null)
                {
                    var fileContent = Encoding.Default.GetBytes(fileField.Value);
                    await _filesService.UploadFile(_genericFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField.FieldName, result.Model.UserId), new MemoryStream(fileContent));
                    result.Images.Add(fileField);
                }
            }

            return result;
        }

        private async Task<TGenericFileAggregatorModel> LoadAndComposeGenericFileAggregatorModel(TGenericModel dataModel)
        {
            var result = Activator.CreateInstance<TGenericFileAggregatorModel>();
            result.Model = dataModel;
            result.Images = new List<FileImageModel>();

            foreach (var fileField in _genericFileAggregatorSettings.FileFields)
            {
                var fileContent = await _filesService.DownloadFile(_genericFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, dataModel.UserId));
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

        private async Task<IEnumerable<TGenericFileAggregatorModel>> LoadAndComposeGenericFileAggregatorModels(IEnumerable<TGenericModel> dataModels)
        {
            var result = new List<TGenericFileAggregatorModel>();
            foreach (var dataModel in dataModels)
            {
                result.Add(await LoadAndComposeGenericFileAggregatorModel(dataModel));
            }

            return result;
        }

        private async Task DeleteItemFiles(string userId)
        {
            foreach (var fileField in _genericFileAggregatorSettings.FileFields)
            {
                await _filesService.DeleteFile(_genericFileAggregatorSettings.ContainerName, FilesService.CreateFileName(fileField, userId));
            }
        }
    }
}