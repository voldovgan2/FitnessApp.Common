using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Services.Generic;

public interface IGenericService<
    TGenericEntity,
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel>
    where TGenericEntity : IGenericEntity
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    Task<TGenericModel> GetItemByUserId(string userId);
    Task<IEnumerable<TGenericModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate);
    Task<IEnumerable<TGenericModel>> GetItems(IEnumerable<string> ids);
    Task<TGenericModel> CreateItem(TCreateGenericModel model);
    Task<TGenericModel> UpdateItem(TUpdateGenericModel model);
    Task<string> DeleteItem(string userId);
}