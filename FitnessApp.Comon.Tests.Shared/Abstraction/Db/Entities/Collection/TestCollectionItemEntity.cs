using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Db.Entities.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Db.Entities.Collection;

[ExcludeFromCodeCoverageAttribute]
public class TestCollectionItemEntity : ICollectionItemEntity, IEquatable<TestCollectionItemEntity>
{
    public string Id { get; set; }
    public string TestProperty { get; set; }

    public static bool operator ==(TestCollectionItemEntity cie1, TestCollectionItemEntity cie2)
    {
        if (cie1 is null)
            return cie2 is null;

        return cie1.Equals(cie2);
    }

    public static bool operator !=(TestCollectionItemEntity cie1, TestCollectionItemEntity cie2)
    {
        return !(cie1 == cie2);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ TestProperty.GetHashCode();
    }

    public override bool Equals(object obj) => IsEqual(obj);

    public bool Equals([AllowNull] TestCollectionItemEntity obj)
    {
        return IsEqual(obj);
    }

    private bool IsEqual(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var cie2 = (TestCollectionItemEntity)obj;
        return Id == cie2.Id && TestProperty == cie2.TestProperty;
    }
}
