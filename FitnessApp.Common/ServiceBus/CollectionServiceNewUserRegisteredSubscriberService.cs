using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Collection;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.ServiceBus;
using FitnessApp.ServiceBus.AzureServiceBus.TopicSubscribers;
using FitnessApp.ServiceBus.Messages;

namespace FitnessApp.Common.ServiceBus
{
    public abstract class CollectionServiceNewUserRegisteredSubscriberService<TCreateModel> : ITopicSubscribers
        where TCreateModel : ICreateCollectionModel
    {
        private readonly Func<TCreateModel, Task<string>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public IEnumerable<Tuple<string, string, Func<string, Task>>> TopicsSubscribers { get; }

        protected CollectionServiceNewUserRegisteredSubscriberService(
            Func<TCreateModel, Task<string>> createItemMethod,
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
            var model = Activator.CreateInstance<TCreateModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}
