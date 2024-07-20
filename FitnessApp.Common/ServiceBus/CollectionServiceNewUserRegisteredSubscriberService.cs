using System;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus;

public abstract class CollectionServiceNewUserRegisteredSubscriberService<TCreateModel>(
    IServiceBus serviceBus,
    Func<TCreateModel, Task<string>> createItemMethod) : MessageBusService(serviceBus)
    where TCreateModel : ICreateCollectionModel
{
    protected override async Task HandleNewUserRegisteredEvent(byte[] data)
    {
        var integrationEvent = JsonSerializer.Deserialize<NewUserRegistered>(data);
        var model = Activator.CreateInstance<TCreateModel>();
        model.UserId = $"ApplicationUser_{integrationEvent.Email}";
        await createItemMethod(model);
    }
}
