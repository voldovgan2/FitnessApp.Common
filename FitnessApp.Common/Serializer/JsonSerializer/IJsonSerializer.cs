using System.Collections.Generic;
using System.Text;

namespace FitnessApp.Common.Serializer.JsonSerializer
{
    public interface IJsonSerializer
    {
        Encoding DefaultEncoding { get; }
        byte[] SerializeToBytes(object data, IEnumerable<string> propertiesToIgnore = null);
        T DeserializeFromBytes<T>(byte[] data);
        string SerializeToString(object data, IEnumerable<string> propertiesToIgnore = null);
        T DeserializeFromString<T>(string data);
    }
}
