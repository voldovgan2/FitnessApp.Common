using System.Collections.Generic;

namespace FitnessApp.Common.Serializer.JsonMapper
{
    public interface IJsonMapper
    {
        T Convert<T>(object from, IEnumerable<string> propertiesToIgnore = null);
    }
}
