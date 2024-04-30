using System;
using NATS.Client;

namespace FitnessApp.Common.ServiceBus.Nats.Services
{
    public class ServiceBus(IConnectionFactory connectionFactory, string url) : IServiceBus
    {
        private readonly IConnection _connection = connectionFactory.CreateConnection(url);

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
