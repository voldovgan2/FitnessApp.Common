using System.Threading.Tasks;

namespace FitnessApp.Common.Vault;

public interface IVaultService
{
    Task<string> GetSecret(string secretKey);
}
