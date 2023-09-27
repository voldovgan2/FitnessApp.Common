using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;

namespace FitnessApp.Common.Abstractions.Services.GenericBlobAggregator
{
    public interface IGenericBlobAggregatorService<TGenericEntity, TGenericBlobAggregatorModel, TGenericModel, TCreateGenericBlobAggregatorModel, TUpdateGenericBlobAggregatorModel>
        where TGenericEntity : IGenericEntity
        where TGenericBlobAggregatorModel : IGenericBlobAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericBlobAggregatorModel : ICreateGenericBlobAggregatorModel
        where TUpdateGenericBlobAggregatorModel : IUpdateGenericBlobAggregatorModel
    {
        Task<TGenericBlobAggregatorModel> CreateItem(TCreateGenericBlobAggregatorModel model);
        Task<TGenericBlobAggregatorModel> UpdateItem(TUpdateGenericBlobAggregatorModel model);
        Task<string> DeleteItem(string userId);
        Task<TGenericBlobAggregatorModel> GetItem(string userId);
        Task<IEnumerable<TGenericBlobAggregatorModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate);
        Task<IEnumerable<TGenericBlobAggregatorModel>> GetItems(string[] ids);
    }
}
