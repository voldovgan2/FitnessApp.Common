using FitnessApp.Abstractions.Db.Entities.Base;
using FitnessApp.Abstractions.Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Db.Repository.Base
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