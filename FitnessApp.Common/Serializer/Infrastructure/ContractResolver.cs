using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FitnessApp.Common.Serializer.Infrastructure
{
    public class ContractResolver : DefaultContractResolver
    {
        private readonly IEnumerable<string> _propertiesToIgnore;

        public ContractResolver(IEnumerable<string> propertiesToIgnore)
        {
            _propertiesToIgnore = propertiesToIgnore ?? Enumerable.Empty<string>();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            properties = properties.Where(p => !_propertiesToIgnore.Contains(p.PropertyName)).ToList();
            return properties;
        }
    }
}
