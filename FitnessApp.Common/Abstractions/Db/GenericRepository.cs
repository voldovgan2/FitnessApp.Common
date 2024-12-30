using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Paged;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Db;

public interface IGenericRepository<
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

public abstract class GenericRepository<
    TGenericEntity,
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel> :
    IGenericRepository<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericEntity : IWithUserIdEntity
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    protected IDbContext<TGenericEntity> DbContext { get; }
    protected IMapper Mapper { get; }

    protected GenericRepository(IDbContext<TGenericEntity> dbContext, IMapper mapper)
    {
        DbContext = dbContext;
        Mapper = mapper;
    }

    public async Task<TGenericModel> GetItemByUserId(string userId)
    {
        var entity = await DbContext.GetByUserId(userId);
        return Mapper.Map<TGenericModel>(entity);
    }

    public async Task<TGenericModel[]> GetItemByUserIds(string[] userIds)
    {
        var items = await DbContext.GetByUserIds(userIds);
        return Mapper.Map<TGenericModel[]>(items);
    }

    public async Task<PagedDataModel<TGenericModel>> GetItems(GetPagedByIdsDataModel model)
    {
        var items = await DbContext.GetByUserIds(model.UserIds);
        return Mapper.Map<TGenericModel[]>(items).ToPaged(model);
    }

    public async Task<TGenericModel> CreateItem(TCreateGenericModel model)
    {
        var entity = Mapper.Map<TGenericEntity>(model);
        var created = await DbContext.CreateItem(entity);
        var result = Mapper.Map<TGenericModel>(created);
        return result;
    }

    public async Task<TGenericModel> UpdateItem(TUpdateGenericModel model)
    {
        var entity = await DbContext.GetByUserId(model.UserId);
        var propertiesToUpdate = model
            .GetType()
            .GetProperties()
            .Where(p => p.GetValue(model, null) != null)
            .Where(p => p.Name != nameof(model.UserId));
        var entityProperties = entity.GetType().GetProperties();
        foreach (var propertyToUpdate in propertiesToUpdate)
        {
            var entityProperty = Array.Find(entityProperties, p => p.Name == propertyToUpdate.Name);
            entityProperty.SetValue(entity, propertyToUpdate.GetValue(model, null));
        }

        var replaced = await DbContext.UpdateItem(entity);
        var result = Mapper.Map<TGenericModel>(replaced);
        return result;
    }

    public async Task<string> DeleteItem(string userId)
    {
        var deleted = await DbContext.DeleteItem(userId);
        var result = deleted.UserId;
        return result;
    }
}