using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class CollectionServiceNewUserRegisteredSubscriberService<TCreateModel>(
        IServiceBus serviceBus,
        Func<TCreateModel, Task<string>> createItemMethod,
        IJsonSerializer serializer
        ) : MessageBusService(serviceBus)
        where TCreateModel : ICreateCollectionModel
    {
        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await createItemMethod(model);
        }
    }
}
