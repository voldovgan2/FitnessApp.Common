using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Repository.Collection;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Abstractions.Services.Validation;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services.Collection
{
    public abstract class CollectionService<
        TCollectionModel,
        TCollectionItemModel,
        TCreateCollectionModel,
        TUpdateCollectionModel>(
            ICollectionRepository<
                TCollectionModel,
                TCollectionItemModel,
                TCreateCollectionModel,
                TUpdateCollectionModel> repository
        ) : ICollectionService<
            TCollectionModel,
            TCollectionItemModel,
            TCreateCollectionModel,
            TUpdateCollectionModel>
        where TCollectionModel : ICollectionModel
        where TCollectionItemModel : ICollectionItemModel
        where TCreateCollectionModel : ICreateCollectionModel
        where TUpdateCollectionModel : IUpdateCollectionModel
    {
        public virtual async Task<TCollectionModel> GetItemByUserId(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await repository.GetItemByUserId(userId);
            return result;
        }

        public virtual async Task<IEnumerable<TCollectionItemModel>> GetCollectionByUserId(string userId, string collectionName)
        {
            List<ValidationError> errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(userId), userId));
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(collectionName), collectionName));
            errors.ThrowIfNotEmpty();

            var result = await repository.GetCollectionByUserId(userId, collectionName);
            return result;
        }

        public virtual async Task<PagedDataModel<TCollectionItemModel>> GetFilteredCollectionItems(string search, GetFilteredCollectionItemsModel<TCollectionItemModel> model)
        {
            List<ValidationError> errors = new List<ValidationError>();
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
            errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.CollectionName), model.CollectionName));
            errors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
            errors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
            errors.ThrowIfNotEmpty();

            PagedDataModel<TCollectionItemModel> result = null;
            var allItems = await repository.GetCollectionByUserId(model.UserId, model.CollectionName);
            if (allItems != null)
            {
                allItems = allItems.Where(model.Predicate);
                result = allItems.ToPaged(model);
            }

            return result;
        }

        public virtual async Task<string> CreateItem(TCreateCollectionModel model)
        {
            var validationErrors = ValidateCreateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await repository.CreateItem(model);
            return result;
        }

        public virtual async Task<TCollectionItemModel> UpdateItem(TUpdateCollectionModel model)
        {
            var validationErrors = ValidateUpdateCollectionModel(model);
            validationErrors.ThrowIfNotEmpty();

            var result = await repository.UpdateItem(model);
            return result;
        }

        public virtual async Task<TCollectionModel> DeleteItem(string userId)
        {
            ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

            var result = await repository.DeleteItem(userId);
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