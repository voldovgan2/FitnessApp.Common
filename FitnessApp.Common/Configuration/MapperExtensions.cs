using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.Common.Configuration;

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
