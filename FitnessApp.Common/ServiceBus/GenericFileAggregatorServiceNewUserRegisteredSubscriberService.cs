using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class GenericFileAggregatorServiceNewUserRegisteredSubscriberService<TGenericFileAggregatorModel, TGenericModel, TCreateGenericFileAggregatorModel>
        : MessageBusService
        where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    {
        private readonly Func<TCreateGenericFileAggregatorModel, Task<TGenericFileAggregatorModel>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        protected GenericFileAggregatorServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateGenericFileAggregatorModel, Task<TGenericFileAggregatorModel>> createItemMethod,
            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericFileAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}