using System;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericFileAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.Common.ServiceBus.Nats.Events;
using FitnessApp.Common.ServiceBus.Nats.Services;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class GenericFileAggregatorServiceNewUserRegisteredSubscriberService<TGenericFileAggregatorModel, TGenericModel, TCreateGenericFileAggregatorModel>(
        IServiceBus serviceBus,
        Func<TCreateGenericFileAggregatorModel, Task<TGenericFileAggregatorModel>> createItemMethod,
        IJsonSerializer serializer
        )
        : MessageBusService(serviceBus)
        where TGenericFileAggregatorModel : IGenericFileAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericFileAggregatorModel : ICreateGenericFileAggregatorModel
    {
        protected override async Task HandleNewUserRegisteredEvent(byte[] data)
        {
            var integrationEvent = serializer.DeserializeFromBytes<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericFileAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await createItemMethod(model);
        }
    }
}