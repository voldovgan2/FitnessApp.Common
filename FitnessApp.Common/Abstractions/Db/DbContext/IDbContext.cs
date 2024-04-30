using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;

namespace FitnessApp.Common.Abstractions.Db.DbContext
{
    public interface IDbContext<TGenericEntity>
        where TGenericEntity : IGenericEntity
    {
        Task<TGenericEntity> GetItemById(string id);
        Task<TGenericEntity> TryGetItemById(string id);
        Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids);
        Task<IEnumerable<TGenericEntity>> GetAllItems(Expression<Func<TGenericEntity, bool>> predicate);
        Task<TGenericEntity> CreateItem(TGenericEntity entity);
        Task<TGenericEntity> UpdateItem(TGenericEntity entity);
        Task UpdateItems(IEnumerable<TGenericEntity> entities);
        Task<TGenericEntity> DeleteItem(string userId);
    }
}