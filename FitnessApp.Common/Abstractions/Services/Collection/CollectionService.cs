using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Abstractions.Services.Search;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.Collection
{
    public abstract class CollectionService<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel>
        : ICollectionService<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel>
        where TCollectionModel : ICollectionModel
        where TCollectionItemModel : ICollectionItemModel
        where TCreateCollectionModel : ICreateCollectionModel
        where TUpdateCollectionModel : IUpdateCollectionModel
    {
        private readonly ICollectionRepository<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel> _repository;
        private readonly ISearchService _searchService;

        protected CollectionService(
            ICollectionRepository<TCollectionModel, TCollectionItemModel, TCreateCollectionModel, TUpdateCollectionModel> repository,
            ISearchService searchService
        )
        {
            _repository = repository;
            _searchService = searchService;
        }

        public virtual async Task<TCollectionModel> GetItemByUserId(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await _repository.GetItemByUserId(userId);
            return result;
        }

        public virtual async Task<IEnumerable<TCollectionItemModel>> GetCollectionByUserId(string userId, string collectionName)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(userId), userId));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(collectionName), collectionName));
            validationErrors.ThrowIfNotEmpty();

            var result = await _repository.GetCollectionByUserId(userId, collectionName);
            return result;
        }

        public virtual async Task<PagedDataModel<TCollectionItemModel>> GetFilteredCollectionItems(string search, GetFilteredCollectionItemsModel<TCollectionItemModel> model)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
            validationErrors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
            validationErrors.ThrowIfNotEmpty();

            PagedDataModel<TCollectionItemModel> result = null;
            var allItems = await _repository.GetCollectionByUserId(model.UserId, model.CollectionName);
            if (allItems != null)
            {
                var itemsBySearch = await _searchService.Search(search);
                allItems = allItems.Where(model.Predicate)
                .Where(i => itemsBySearch.Contains(i.Id));
                result = allItems.ToPaged(model);
            }

            return result;
        }

        public virtual async Task<string> CreateItem(TCreateCollectionModel model)
        {
            var validationErrors = ValidateCreateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await _repository.CreateItem(model);
            return result;
        }

        public virtual async Task<TCollectionItemModel> UpdateItem(TUpdateCollectionModel model)
        {
            var validationErrors = ValidateUpdateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await _repository.UpdateItem(model);
            return result;
        }

        public virtual async Task<TCollectionModel> DeleteItem(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await _repository.DeleteItem(userId);
            return result;
        }

        protected virtual IEnumerable<ValidationError> ValidateCreateCollectionModel(TCreateCollectionModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            return errors;
        }

        protected virtual IEnumerable<ValidationError> ValidateUpdateCollectionModel(TUpdateCollectionModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
            errors.AddRange(ValidateCollectionItemModel(model));
            return errors;
        }

        protected virtual IEnumerable<ValidationError> ValidateCollectionItemModel(TUpdateCollectionModel model)
        {
            var errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.Model.Id), model.Model?.Id));
            return errors;
        }
    }
}