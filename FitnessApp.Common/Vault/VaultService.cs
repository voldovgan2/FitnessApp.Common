using System.Threading.Tasks;
using VaultSharp;

namespace FitnessApp.Common.Vault
{
    public class VaultService : IVaultService
    {
        private readonly IVaultClient _vaultClient;

        public VaultService(IVaultClient vaultClient)
        {
            _vaultClient = vaultClient;
        }

        public async Task<string> GetSecret(string secretKey)
        {
            var response = await _vaultClient.V1.Secrets.KeyValue.V1.ReadSecretAsync("fitness-app");
            return response.Data[secretKey].ToString();
        }
    }
}
