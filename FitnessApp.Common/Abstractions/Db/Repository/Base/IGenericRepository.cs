using FitnessApp.Common.Abstractions.Db.Entities.Base;
using FitnessApp.Common.Abstractions.Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Db.Repository.Base
{
    public interface IGenericRepository<Entity, Model, CreateModel, UpdateModel>
        where Entity: IEntity
        where Model : ISearchableModel
        where CreateModel: ICreateModel
        where UpdateModel: IUpdateModel
    {
        Task<IEnumerable<Model>> GetAllItemsAsync();
        Task<Model> GetItemByUserIdAsync(string userId);
        Task<Model> CreateItemAsync(CreateModel model);
        Task<Model> UpdateItemAsync(UpdateModel model);
        Task<string> DeleteItemAsync(string userId);
    }
}