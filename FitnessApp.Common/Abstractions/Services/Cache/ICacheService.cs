using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Services.Cache
{
    public interface ICacheService<Model>
    {
        Task<Model> GetItem(string id);
        Task SaveItem(string id, Model model);        
        Task DeleteItem(string id);
    }
}