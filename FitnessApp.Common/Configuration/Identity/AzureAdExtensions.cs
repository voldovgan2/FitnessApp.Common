using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Configuration.Identity
{
    public static class AzureAdExtensions
    {
        public static IServiceCollection ConfigureAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                 {
                     options.Audience = configuration["AzureAd:Audience"];
                     options.Authority = configuration["AzureAd:Authority"];
                     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                     {
                         ValidAudience = configuration["AzureAd:ValidAudience"],
                         ValidIssuer = configuration["AzureAd:ValidIssuer"]
                     };
                 });
            return services;
        }
    }
}