using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public class GenericServiceNewUserRegisteredSubscriberService<TGenericModel, TCreateGenericModel> : MessageBusService
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
    {
        private readonly Func<TCreateGenericModel, Task<TGenericModel>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public GenericServiceNewUserRegisteredSubscriberService(
            IServiceBus serviceBus,
            Func<TCreateGenericModel, Task<TGenericModel>> createItemMethod,
            IJsonSerializer serializer
        ) : base(serviceBus)
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
        }

        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = _serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}
