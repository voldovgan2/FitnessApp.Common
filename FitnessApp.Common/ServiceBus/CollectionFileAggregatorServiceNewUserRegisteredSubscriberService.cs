using System;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.CollectionFileAggregator;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus;

public class CollectionFileAggregatorServiceNewUserRegisteredSubscriberService<TCreateCollectionFileAggregatorModel>(
    IServiceBus serviceBus,
    Func<TCreateCollectionFileAggregatorModel, Task<string>> createItemMethod) : MessageBusService(serviceBus)
    where TCreateCollectionFileAggregatorModel : ICreateCollectionFileAggregatorModel
{
    protected override async Task HandleNewUserRegisteredEvent(byte[] data)
    {
        var integrationEvent = JsonSerializer.Deserialize<NewUserRegistered>(data);
        var model = Activator.CreateInstance<TCreateCollectionFileAggregatorModel>();
        model.UserId = $"ApplicationUser_{integrationEvent.Email}";
        await createItemMethod(model);
    }
}