using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FitnessApp.Common.Serializer
{
    public class ContractResolver(IEnumerable<string> propertiesToIgnore) : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            properties = properties.Where(p => !propertiesToIgnore.Contains(p.PropertyName)).ToList();
            return properties;
        }
    }
}
