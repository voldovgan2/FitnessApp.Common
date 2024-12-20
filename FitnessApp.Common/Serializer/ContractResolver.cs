﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FitnessApp.Common.Serializer;

[ExcludeFromCodeCoverage]
public class ContractResolver(string[] propertiesToIgnore) : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var properties = base.CreateProperties(type, memberSerialization);
        properties = properties.Where(p => !propertiesToIgnore.Contains(p.PropertyName)).ToList();
        return properties;
    }
}
