using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FitnessApp.Common.Abstractions.Db.DbContext;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Paged.Extensions;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Db.Repository.Generic;

public abstract class GenericRepository<
    TGenericEntity,
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel> :
    IGenericRepository<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericEntity : IGenericEntity
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
        var entity = await DbContext.GetItemById(userId);
        return Mapper.Map<TGenericModel>(entity);
    }

    public async Task<IEnumerable<TGenericModel>> GetItemsByIds(IEnumerable<string> ids)
    {
        var items = await DbContext.GetItemsByIds(ids);
        return Mapper.Map<IEnumerable<TGenericModel>>(items);
    }

    public async Task<PagedDataModel<TGenericModel>> GetItemsByIds(GetPagedByIdsDataModel model)
    {
        var items = await DbContext.GetItemsByIds(model.Ids);
        return Mapper.Map<IEnumerable<TGenericModel>>(items).ToPaged(model);
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
        var entity = await DbContext.GetItemById(model.UserId);
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
        string result = deleted.UserId;
        return result;
    }
}