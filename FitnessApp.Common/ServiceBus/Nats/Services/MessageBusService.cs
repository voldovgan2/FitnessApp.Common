using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NATS.Client;

namespace FitnessApp.Common.ServiceBus.Nats.Services
{
    public abstract class MessageBusService : IHostedService
    {
        private readonly IServiceBus _serviceBus;
        private IAsyncSubscription _eventSubscription;

        protected MessageBusService(IServiceBus serviceBus)
        {
            _serviceBus = serviceBus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _eventSubscription = _serviceBus.SubscribeEvent(Topic.NEW_USER_REGISTERED, (sender, args) =>
            {
                HandleNewUserRegisteredEvent(args.Message.Data);
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _eventSubscription.Unsubscribe();
            return Task.CompletedTask;
        }

        protected abstract Task HandleNewUserRegisteredEvent(byte[] data);
    }
}
