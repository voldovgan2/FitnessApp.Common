using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FitnessApp.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FitnessApp.Common.Serializer
{
    public class SensitiveDataContractResolver : DefaultContractResolver
    {
        public bool HasSensitiveProperties(Type type)
        {
            return GetSensitiveProperties(type).Any();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var sensitiveProperties = GetSensitiveProperties(type)
                .Select(p => p.Name);
            return base
                .CreateProperties(type, memberSerialization)
                .Where(p => !sensitiveProperties.Contains(p.PropertyName))
                .ToList();
        }

        private IEnumerable<PropertyInfo> GetSensitiveProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(SensitiveAttribute), false).Any());
        }
    }
}
