using System;
using NATS.Client;

namespace FitnessApp.Common.ServiceBus.Nats.Services;

public interface IServiceBus
{
    void PublishEvent(string subject, byte[] data);
    IAsyncSubscription SubscribeEvent(string subject, EventHandler<MsgHandlerEventArgs> handler);
    void UnsubscribeEvent(IAsyncSubscription subscription);
}
