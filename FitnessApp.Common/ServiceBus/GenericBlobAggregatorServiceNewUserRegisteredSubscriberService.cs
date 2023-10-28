using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class GenericBlobAggregatorServiceNewUserRegisteredSubscriberService<TGenericBlobAggregatorModel, TGenericModel, TCreateGenericBlobAggregatorModel>
        : MessageBusService
        where TGenericBlobAggregatorModel : IGenericBlobAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericBlobAggregatorModel : ICreateGenericBlobAggregatorModel
    {
        private readonly Func<TCreateGenericBlobAggregatorModel, Task<TGenericBlobAggregatorModel>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        protected GenericBlobAggregatorServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateGenericBlobAggregatorModel, Task<TGenericBlobAggregatorModel>> createItemMethod,
            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericBlobAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}