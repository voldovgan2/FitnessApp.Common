using System.Collections.Generic;
using System.Text;

namespace FitnessApp.Common.Serializer.JsonSerializer
{
    public interface IJsonSerializer
    {
        Encoding DefaultEncoding { get; }
        byte[] SerializeToBytes(object data);
        byte[] SerializeToBytes(object data, IEnumerable<string> propertiesToIgnore);
        T DeserializeFromBytes<T>(byte[] data);
        string SerializeToString(object data);
        string SerializeToString(object data, IEnumerable<string> propertiesToIgnore);
        T DeserializeFromString<T>(string data);
    }
}
