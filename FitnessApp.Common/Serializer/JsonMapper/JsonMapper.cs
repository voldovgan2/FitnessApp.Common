using FitnessApp.Common.Serializer.Infrastructure;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FitnessApp.Common.Serializer.JsonMapper
{
    public class JsonMapper : IJsonMapper
    {
        public T Convert<T>(object from, IEnumerable<string> propertiesToIgnore = null)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(from, new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver(propertiesToIgnore)
            }));
        }
    }
}
