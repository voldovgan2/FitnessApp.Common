using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FitnessApp.Common.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FitnessApp.Common.Serializer;

[ExcludeFromCodeCoverage]
public class SensitiveDataContractResolver : DefaultContractResolver
{
    public static bool HasSensitiveProperties(Type type)
    {
        return GetSensitiveProperties(type).Length != 0;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var sensitiveProperties = GetSensitiveProperties(type)
            .Select(p => p.Name);
        return [
            ..base
                .CreateProperties(type, memberSerialization)
                .Where(p => !sensitiveProperties.Contains(p.PropertyName))
        ];
    }

    private static PropertyInfo[] GetSensitiveProperties(Type type)
    {
        return [
            ..type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes(typeof(SensitiveAttribute), false).Length != 0)
        ];
    }
}
