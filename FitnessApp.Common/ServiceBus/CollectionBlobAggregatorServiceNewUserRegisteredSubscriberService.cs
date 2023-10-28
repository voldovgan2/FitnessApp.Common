using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.CollectionBlobAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public class CollectionBlobAggregatorServiceNewUserRegisteredSubscriberService<TCreateCollectionBlobAggregatorModel> : MessageBusService
        where TCreateCollectionBlobAggregatorModel : ICreateCollectionBlobAggregatorModel
    {
        private readonly Func<TCreateCollectionBlobAggregatorModel, Task<string>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public CollectionBlobAggregatorServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateCollectionBlobAggregatorModel, Task<string>> createItemMethod,
            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateCollectionBlobAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}