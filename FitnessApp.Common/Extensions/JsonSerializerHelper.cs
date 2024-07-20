using System.Text;
using System.Text.Json;

namespace FitnessApp.Common.Extensions;

public static class JsonSerializerHelper
{
    public static Encoding DefaultEncoding { get; set; } = Encoding.UTF32;

    public static byte[] SerializeToBytes(object data)
    {
        return DefaultEncoding.GetBytes(JsonSerializer.Serialize(data));
    }

    public static T DeserializeFromBytes<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(DefaultEncoding.GetString(data));
    }
}
