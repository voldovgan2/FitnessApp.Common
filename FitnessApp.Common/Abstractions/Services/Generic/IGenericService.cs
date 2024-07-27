using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Common.Abstractions.Services.Generic;

public interface IGenericService<
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    Task<TGenericModel> GetItemByUserId(string userId);
    Task<IEnumerable<TGenericModel>> GetItemsByIds(IEnumerable<string> ids);
    Task<TGenericModel> CreateItem(TCreateGenericModel model);
    Task<TGenericModel> UpdateItem(TUpdateGenericModel model);
    Task<string> DeleteItem(string userId);
}