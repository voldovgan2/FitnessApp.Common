using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FitnessApp.Common.Configuration;

public static class IdentityExtensions
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication("Bearer")
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.Authority = configuration["OpenIdConnect:Issuer"];
                cfg.Audience = configuration["OpenIdConnect:Audience"];
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = configuration["OpenIdConnect:Audience"],
                    ValidIssuer = configuration["OpenIdConnect:Issuer"]
                };
            });
        return services;
    }
}