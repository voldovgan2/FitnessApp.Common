using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitnessApp.Common.Abstractions.Models.Generic;
using FitnessApp.Common.Serializer.JsonSerializer;
using FitnessApp.ServiceBus;
using FitnessApp.ServiceBus.AzureServiceBus.TopicSubscribers;
using FitnessApp.ServiceBus.Messages;

namespace FitnessApp.Common.ServiceBus
{
    public class GenericServiceNewUserRegisteredSubscriberService<TGenericModel, TCreateGenericModel> : ITopicSubscribers
        where TGenericModel : IGenericModel
        where TCreateGenericModel : ICreateGenericModel
    {
        private readonly Func<TCreateGenericModel, Task<TGenericModel>> _createItemMethod;
        private readonly IJsonSerializer _serializer;

        public IEnumerable<Tuple<string, string, Func<string, Task>>> TopicsSubscribers { get; }

        public GenericServiceNewUserRegisteredSubscriberService(
            Func<TCreateGenericModel, Task<TGenericModel>> createItemMethod,
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
            var model = Activator.CreateInstance<TCreateGenericModel>();
            model.UserId = $"ApplicationUser_{integrationEvent.Email}";
            await _createItemMethod(model);
        }
    }
}
