using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus;

[ExcludeFromCodeCoverage]
public abstract class GenericFileAggregatorServiceNewUserRegisteredSubscriberService<
    TGenericFileAggregatorModel,
    TGenericModel,
    TCreateGenericFileAggregatorModel>(
        IServiceBus serviceBus,
        Func<TCreateGenericFileAggregatorModel,
        Task<TGenericFileAggregatorModel>> createItemMethod) : MessageBusService(serviceBus)
    where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
    where TGenericModel : IGenericModel
    where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
{
    protected override async Task HandleNewUserRegisteredEvent(byte[] data)
    {
        var integrationEvent = JsonSerializer.Deserialize<NewUserRegistered>(data);
        var model = Activator.CreateInstance<TCreateGenericFileAggregatorModel>();
        model.UserId = $"ApplicationUser_{integrationEvent.Email}";
        await createItemMethod(model);
    }
}