using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db;
using FitnessApp.Common.Abstractions.Extensions;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Helpers;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Services;

public interface IGenericService<
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    Task<TGenericModel> GetItemByUserId(string userId);
    Task<TGenericModel[]> GetItemByUserIds(string[] userIds);
    Task<PagedDataModel<TGenericModel>> GetItems(GetPagedByIdsDataModel model);
    Task<TGenericModel> CreateItem(TCreateGenericModel model);
    Task<TGenericModel> UpdateItem(TUpdateGenericModel model);
    Task<string> DeleteItem(string userId);
}

public abstract class GenericService<
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel> :
    IGenericService<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    protected IGenericRepository<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel> Repository
    { get; }

    protected GenericService(IGenericRepository<TGenericModel, TCreateGenericModel, TUpdateGenericModel> repository)
    {
        Repository = repository;
    }

    public async Task<TGenericModel> GetItemByUserId(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var result = await Repository.GetItemByUserId(userId);
        return result;
    }

    public async Task<TGenericModel[]> GetItemByUserIds(string[] userIds)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyIdsField(nameof(userIds), userIds);

        var result = await Repository.GetItemByUserIds(userIds);
        return result;
    }

    public async Task<PagedDataModel<TGenericModel>> GetItems(GetPagedByIdsDataModel model)
    {
        var errors = new List<ValidationError>();
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyIdsField(nameof(model.UserIds), model.UserIds);
        errors.AddIfNotNull(ValidationHelper.ValidateRange(0, int.MaxValue, model.Page, nameof(model.Page)));
        errors.AddIfNotNull(ValidationHelper.ValidateRange(1, int.MaxValue, model.PageSize, nameof(model.PageSize)));
        errors.ThrowIfNotEmpty();

        var result = await Repository.GetItems(model);
        return result;
    }

    public async Task<TGenericModel> CreateItem(TCreateGenericModel model)
    {
        var validationErrors = ValidateCreateGenericModel(model);
        validationErrors.ThrowIfNotEmpty();

        var result = await Repository.CreateItem(model);
        return result;
    }

    public async Task<TGenericModel> UpdateItem(TUpdateGenericModel model)
    {
        var validationErrors = ValidateUpdateGenericModel(model);
        validationErrors.ThrowIfNotEmpty();

        var result = await Repository.UpdateItem(model);
        return result;
    }

    public async Task<string> DeleteItem(string userId)
    {
        ValidationHelper.ThrowExceptionIfNotValidatedEmptyStringField(nameof(userId), userId);

        var result = await Repository.DeleteItem(userId);
        return result;
    }

    protected virtual List<ValidationError> ValidateCreateGenericModel(TCreateGenericModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        return errors;
    }

    protected virtual List<ValidationError> ValidateUpdateGenericModel(TUpdateGenericModel model)
    {
        var errors = new List<ValidationError>();
        errors.AddIfNotNull(ValidationHelper.ValidateEmptyStringField(nameof(model.UserId), model.UserId));
        return errors;
    }
}