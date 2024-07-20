using System.Threading.Tasks;
using VaultSharp;

namespace FitnessApp.Common.Vault;

public class VaultService(IVaultClient vaultClient) : IVaultService
{
    public async Task<string> GetSecret(string secretKey)
    {
        var response = await vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync("fitness-app");
        return response.Data[secretKey].ToString();
    }
}
