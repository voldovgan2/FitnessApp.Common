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
    TUpdateGenericModel>
    (IDbContext<TGenericEntity> dbContext, IMapper mapper) :
    IGenericRepository<
        TGenericModel,
        TCreateGenericModel,
        TUpdateGenericModel>
    where TGenericEntity : IGenericEntity
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    public async Task<TGenericModel> GetItemByUserId(string userId)
    {
        var entity = await dbContext.GetItemById(userId);
        return mapper.Map<TGenericModel>(entity);
    }

    public async Task<IEnumerable<TGenericModel>> GetItemsByIds(IEnumerable<string> ids)
    {
        var items = await dbContext.GetItemsByIds(ids);
        return mapper.Map<IEnumerable<TGenericModel>>(items);
    }

    public async Task<PagedDataModel<TGenericModel>> GetItemsByIds(GetPagedByIdsDataModel model)
    {
        var items = await dbContext.GetItemsByIds(model.Ids);
        return mapper.Map<IEnumerable<TGenericModel>>(items).ToPaged(model);
    }

    public async Task<TGenericModel> CreateItem(TCreateGenericModel model)
    {
        var entity = mapper.Map<TGenericEntity>(model);
        var created = await dbContext.CreateItem(entity);
        var result = mapper.Map<TGenericModel>(created);
        return result;
    }

    public async Task<TGenericModel> UpdateItem(TUpdateGenericModel model)
    {
        var entity = await dbContext.GetItemById(model.UserId);
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

        var replaced = await dbContext.UpdateItem(entity);
        var result = mapper.Map<TGenericModel>(replaced);
        return result;
    }

    public async Task<string> DeleteItem(string userId)
    {
        var deleted = await dbContext.DeleteItem(userId);
        string result = deleted.UserId;
        return result;
    }
}