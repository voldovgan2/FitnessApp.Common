using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Paged.Models.Input;
using FitnessApp.Common.Paged.Models.Output;

namespace FitnessApp.Common.Abstractions.Db.Repository.Generic;

public interface IGenericRepository<
    TGenericModel,
    TCreateGenericModel,
    TUpdateGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericModel : ICreateGenericModel
    where TUpdateGenericModel : IUpdateGenericModel
{
    Task<TGenericModel> GetItemByUserId(string userId);
    Task<IEnumerable<TGenericModel>> GetItemsByIds(IEnumerable<string> ids);
    Task<PagedDataModel<TGenericModel>> GetItemsByIds(GetPagedByIdsDataModel model);
    Task<TGenericModel> CreateItem(TCreateGenericModel model);
    Task<TGenericModel> UpdateItem(TUpdateGenericModel model);
    Task<string> DeleteItem(string userId);
}