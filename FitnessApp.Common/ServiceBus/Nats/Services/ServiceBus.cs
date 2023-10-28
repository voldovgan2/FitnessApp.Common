using System;
using FitnessApp.Common.Configuration.Nats;
using Microsoft.Extensions.Options;
using NATS.Client;

namespace FitnessApp.Common.ServiceBus.Nats.Services
{
    public class ServiceBus : IServiceBus
    {
        private readonly IConnection _connection = null;

        public ServiceBus(
            IConnectionFactory connectionFactory,
            IOptions<ServiceBusSettings> settings)
        {
            _connection = connectionFactory.CreateConnection(settings.Value.Url);
        }

        public void PublishEvent(string subject, byte[] data)
        {
            _connection.Publish(subject, data);
        }

        public IAsyncSubscription SubscribeEvent(string subject, EventHandler<MsgHandlerEventArgs> handler)
        {
            return _connection.SubscribeAsync(subject, handler);
        }

        public void UnsubscribeEvent(IAsyncSubscription subscription)
        {
            subscription.Unsubscribe();
        }
    }
}
