using System.Collections.Generic;
using System.Text;
using FitnessApp.Common.Serializer.Infrastructure;
using Newtonsoft.Json;

namespace FitnessApp.Common.Serializer.JsonSerializer
{
    public class JsonSerializer : IJsonSerializer
    {
        public Encoding DefaultEncoding { get; set; } = Encoding.UTF32;

        public byte[] SerializeToBytes(object data, IEnumerable<string> propertiesToIgnore = null)
        {
            return DefaultEncoding.GetBytes(JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver(propertiesToIgnore)
            }));
        }

        public T DeserializeFromBytes<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(DefaultEncoding.GetString(data));
        }

        public string SerializeToString(object data, IEnumerable<string> propertiesToIgnore = null)
        {
            return JsonConvert.SerializeObject(data, new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver(propertiesToIgnore)
            });
        }

        public T DeserializeFromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
