using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public class CollectionFileAggregatorServiceNewUserRegisteredSubscriberService<TCreateCollectionFileAggregatorModel> : MessageBusService
        where TCreateCollectionFileAggregatorModel : ICreateCollectionFileAggregatorModel
    {
        private readonly Func<TCreateCollectionFileAggregatorModel, Task<string>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public CollectionFileAggregatorServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateCollectionFileAggregatorModel, Task<string>> createItemMethod,
            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateCollectionFileAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}