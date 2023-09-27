using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Abstractions.Models.GenericBlobAggregator;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.ServiceBus;
using FitnessApp.ServiceBus.AzureServiceBus.TopicSubscribers;
using FitnessApp.ServiceBus.Messages;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class GenericBlobAggregatorServiceNewUserRegisteredSubscriberService<TGenericBlobAggregatorModel, TGenericModel, TCreateGenericBlobAggregatorModel>
        : ITopicSubscribers
        where TGenericBlobAggregatorModel : IGenericBlobAggregatorModel<TGenericModel>
        where TGenericModel : IGenericModel
        where TCreateGenericBlobAggregatorModel : ICreateGenericBlobAggregatorModel
    {
        private readonly Func<TCreateGenericBlobAggregatorModel, Task<TGenericBlobAggregatorModel>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public IEnumerable<Tuple<string, string, Func<string, Task>>> TopicsSubscribers { get; }

        protected GenericBlobAggregatorServiceNewUserRegisteredSubscriberService(
            Func<TCreateGenericBlobAggregatorModel, Task<TGenericBlobAggregatorModel>> createItemMethod,
            string subscription,
            IJsonSerializer serializer
        )
        {
            _createItemMethod = createItemMethod;
            _serializer = serializer;
            TopicsSubscribers = new List<Tuple<string, string, Func<string, Task>>>
            {
                new Tuple<string, string, Func<string, Task>>(Topic.NEW_USER_REGISTERED, subscription, HandleNewUserRegisteredEvent)
            };
        }

        protected virtual async Task HandleNewUserRegisteredEvent(string data)
        {
            var integrationEvent = _serializer.DeserializeFromString<NewUserRegistered>(data);
            var model = Activator.CreateInstance<TCreateGenericBlobAggregatorModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}