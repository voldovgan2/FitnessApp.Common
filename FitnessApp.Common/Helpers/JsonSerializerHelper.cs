using System.Diagnostics.CodeAnalysis;
using System.Text;
using FitnessApp.Common.Serializer;
using Newtonsoft.Json;

namespace FitnessApp.Common.Helpers;

[ExcludeFromCodeCoverage]
public static class JsonSerializerHelper
{
    private static Encoding DefaultEncoding { get; set; } = Encoding.UTF32;

    public static byte[] SerializeToBytes(object data)
    {
        return DefaultEncoding.GetBytes(JsonConvert.SerializeObject(data));
    }

    public static byte[] SerializeToBytes(object data, string[] propertiesToIgnore)
    {
        return DefaultEncoding.GetBytes(JsonConvert.SerializeObject(data, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolver(propertiesToIgnore)
        }));
    }

    public static T DeserializeFromBytes<T>(byte[] data)
    {
        return JsonConvert.DeserializeObject<T>(DefaultEncoding.GetString(data));
    }

    public static string SerializeToString(object data, string[] propertiesToIgnore)
    {
        return JsonConvert.SerializeObject(data, new JsonSerializerSettings
        {
            ContractResolver = new ContractResolver(propertiesToIgnore)
        });
    }
}
