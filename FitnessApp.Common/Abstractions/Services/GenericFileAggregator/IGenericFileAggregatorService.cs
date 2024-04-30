using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;

namespace FitnessApp.Common.Abstractions.Services.GenericFileAggregator
{
    public interface IGenericFileAggregatorService<
        TGenericEntity,
        TGenericFileAggregatorModel,
        TGenericModel,
        TCreateGenericFileAggregatorModel,
        TUpdateGenericFileAggregatorModel>
        where TGenericEntity : IGenericEntity
        where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
        where TUpdateGenericFileAggregatorModel : IUpdateGenericFileAggregatorModel
    {
        Task<TGenericFileAggregatorModel> CreateItem(TCreateGenericFileAggregatorModel model);
        Task<TGenericFileAggregatorModel> UpdateItem(TUpdateGenericFileAggregatorModel model);
        Task<string> DeleteItem(string userId);
        Task<TGenericFileAggregatorModel> GetItem(string userId);
        Task<IEnumerable<TGenericFileAggregatorModel>> GetItems(string search, Expression<Func<TGenericEntity, bool>> predicate);
        Task<IEnumerable<TGenericFileAggregatorModel>> GetItems(string[] ids);
    }
}
