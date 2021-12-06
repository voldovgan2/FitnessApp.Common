using FitnessApp.Abstractions.Db.Entities.Base;
using FitnessApp.Abstractions.Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Services.Base
{
    public interface IGenericService<Entity, Model, GetItemsModel, CreateModel, UpdateModel>
        where Entity : IEntity
        where Model : ISearchableModel
        where GetItemsModel : IGetItemsModel
        where CreateModel : ICreateModel
        where UpdateModel : IUpdateModel
    {
        Task<Model> GetItemByUserIdAsync(string userId);
        Task<IEnumerable<Model>> GetItemsAsync(GetItemsModel model);
        Task<Model> CreateItemAsync(CreateModel model);
        Task<Model> UpdateItemAsync(UpdateModel model);
        Task<string> DeleteItemAsync(string userId);
    }
}