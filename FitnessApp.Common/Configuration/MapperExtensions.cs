using System;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Configuration;

[ExcludeFromCodeCoverage]
public static class MapperExtensions
{
    public static IServiceCollection ConfigureMapper(this IServiceCollection services, Profile profile)
    {
        ArgumentNullException.ThrowIfNull(services);

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(profile);
        });
        services.AddSingleton(mapperConfig.CreateMapper());

        return services;
    }
}
