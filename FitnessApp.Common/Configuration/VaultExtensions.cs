using System;
using FitnessApp.Common.Vault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace FitnessApp.Common.Configuration
{
    public static class VaultExtensions
    {
        public static IServiceCollection ConfigureVault(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddTransient<IVaultClient, VaultClient>(
                sp =>
                {
                    var authMethod = new TokenAuthMethodInfo(configuration.GetValue<string>("VaultClient:Token"));
                    var address = configuration.GetValue<string>("VaultClient:Address");
                    var config = new VaultClientSettings(address, authMethod);
                    var vaultClient = new VaultClient(config);
                    return vaultClient;
                }
            );

            services.AddTransient<IVaultService, VaultService>();

            return services;
        }
    }
}
