using System;
using FitnessApp.Common.ServiceBus.Nats.Services;
using NATS.Client;

namespace FitnessApp.Common.Tests.Fixtures;

public class MockServiceBus : IServiceBus
{
    public MockServiceBus(IConnectionFactory connectionFactory)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
    }

    public void PublishEvent(string subject, byte[] data)
    { }

    public IAsyncSubscription SubscribeEvent(string subject, EventHandler<MsgHandlerEventArgs> handler)
    {
        throw new NotImplementedException();
    }

    public void UnsubscribeEvent(IAsyncSubscription subscription)
    { }
}