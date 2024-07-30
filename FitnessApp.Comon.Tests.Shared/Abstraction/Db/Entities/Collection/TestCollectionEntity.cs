using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using MongoDB.Bson.Serialization.Attributes;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;

[ExcludeFromCodeCoverageAttribute]
public class TestCollectionEntity : ICollectionEntity, IEquatable<TestCollectionEntity>
{
    [BsonId]
    public string UserId { get; set; }
    public Dictionary<string, List<ICollectionItemEntity>> Collection { get; set; }

    public static bool operator ==(TestCollectionEntity ce1, TestCollectionEntity ce2)
    {
        if (ce1 is null)
            return ce2 is null;

        return ce1.Equals(ce2);
    }

    public static bool operator !=(TestCollectionEntity ce1, TestCollectionEntity ce2)
    {
        return !(ce1 == ce2);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return IsEqual(obj);
    }

    public bool Equals([AllowNull] TestCollectionEntity obj)
    {
        return IsEqual(obj);
    }

    private bool IsEqual(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var ce2 = (TestCollectionEntity)obj;
        return UserId == ce2.UserId;
    }
}
