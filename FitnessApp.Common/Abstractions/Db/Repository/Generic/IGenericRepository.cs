using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Db.Entities.Generic;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Db.Repository.Generic
{
    public interface IGenericRepository<TGenericEntity, TGenericModel, TCreateGenericModel, TUpdateGenericModel>
        where TGenericEntity : IGenericEntity
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
        where TUpdateGenericModel : IUpdateGenericModel
    {
        Task<IQueryable<TGenericEntity>> GetAllItems();
        Task<TGenericModel> GetItemByUserId(string userId);
        Task<IEnumerable<TGenericEntity>> GetItemsByIds(IEnumerable<string> ids);
        Task<TGenericModel> CreateItem(TCreateGenericModel model);
        Task<TGenericModel> UpdateItem(TUpdateGenericModel model);
        Task<string> DeleteItem(string userId);
    }
}