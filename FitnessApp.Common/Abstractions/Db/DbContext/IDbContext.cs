using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Paged.Models.Input;

namespace FitnessApp.Common.Abstractions.Db.DbContext;

public interface IDbContext<TGenericEntity>
    where TGenericEntity : IGenericEntity
{
    Task<TGenericEntity> GetItemById(string id);
    Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids);
    Task<TGenericEntity> CreateItem(TGenericEntity entity);
    Task<TGenericEntity> UpdateItem(TGenericEntity entity);
    Task<TGenericEntity> DeleteItem(string id);
}