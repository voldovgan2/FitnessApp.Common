﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Db.Repository.Generic;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Abstractions.Services.Validation;

namespace FitnessApp.Common.Abstractions.Services.Generic
{
    public abstract class GenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel>
        : IGenericService<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel>
        where TGenericEntity : IGenericEntity
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
        where TUpdateGenericModel : IUpdateGenericModel
    {
        private readonly IGenericRepository<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> _repository;
        private readonly IMapper _mapper;

        protected GenericService(
            IGenericRepository<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel> repository,
            IMapper mapper
        )
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TGenericModel> GetItemByUserId(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await _repository.GetItemByUserId(userId);
            return result;
        }

        public async Task<IEnumerable<TGenericModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate)
        {
            var data = (await _repository.GetAllItems())
                .Where(predicate)
                .ToList();
            var result = _mapper.Map<List<TGenericModel>>(data);
            return result;
        }

        public async Task<IEnumerable<TGenericModel>> GetItems(IEnumerable<string> ids)
        {
            var data = await _repository.GetItemsByIds(ids.Where(id => !string.IsNullOrWhiteSpace(id)));
            var result = data.Select(entity => _mapper.Map<TGenericModel>(entity));
            return result;
        }

        public async Task<TGenericModel> CreateItem(TCreateGenericModel model)
        {
            var validationErrors = ValidateCreateGenericModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await _repository.CreateItem(model);
            return result;
        }

        public async Task<TGenericModel> UpdateItem(TUpdateGenericModel model)
        {
            var validationErrors = ValidateUpdateGenericModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await _repository.UpdateItem(model);
            return result;
        }

        public async Task<string> DeleteItem(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await _repository.DeleteItem(userId);
            return result;
        }

        protected virtual IEnumerable<ValidationError> ValidateCreateGenericModel(TCreateGenericModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            return errors;
        }

        protected virtual IEnumerable<ValidationError> ValidateUpdateGenericModel(TUpdateGenericModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            return errors;
        }
    }
}