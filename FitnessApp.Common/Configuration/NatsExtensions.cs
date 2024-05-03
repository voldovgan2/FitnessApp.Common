using System;
using FitnessApp.Common.ServiceBus.Nats.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

namespace FitnessApp.Common.Configuration
{
    public static class NatsExtensions
    {
        public static IServiceCollection ConfigureNats(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddTransient<IConnectionFactory, ConnectionFactory>();
            services.AddTransient<IServiceBus, ServiceBus.Nats.Services.ServiceBus>(
                sp =>
                {
                    var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
                    var url = configuration.GetValue<string>("ServiceBus:url");
                    return new ServiceBus.Nats.Services.ServiceBus(connectionFactory, url);
                }
            );

            return services;
        }
    }
}
