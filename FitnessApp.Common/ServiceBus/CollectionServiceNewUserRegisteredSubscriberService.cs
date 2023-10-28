using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class CollectionServiceNewUserRegisteredSubscriberService<TCreateModel> : MessageBusService
        where TCreateModel : ICreateCollectionModel
    {
        private readonly Func<TCreateModel, Task<string>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        protected CollectionServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateModel, Task<string>> createItemMethod,

            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}
