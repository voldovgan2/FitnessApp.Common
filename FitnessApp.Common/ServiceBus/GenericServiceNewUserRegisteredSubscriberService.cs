using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public class GenericServiceNewUserRegisteredSubscriberService<TGenericModel, TCreateGenericModel>(
        IServiceBus serviceBus,
        Func<TCreateGenericModel, Task<TGenericModel>> createItemMethod,
        IJsonSerializer serializer
        ) : MessageBusService(serviceBus)
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
    {
        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await createItemMethod(model);
        }
    }
}
