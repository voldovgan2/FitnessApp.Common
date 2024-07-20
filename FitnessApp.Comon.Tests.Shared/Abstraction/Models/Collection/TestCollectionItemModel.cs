using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Collection;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Collection;

public class TestCollectionItemModel : ICollectionItemModel, IEquatable<TestCollectionItemModel>
{
    public string Id { get; set; }
    public string TestProperty { get; set; }

    public static bool operator ==(TestCollectionItemModel ge1, TestCollectionItemModel ge2)
    {
        if (ge1 is null)
            return ge2 is null;

        return ge1.Equals(ge2);
    }

    public static bool operator !=(TestCollectionItemModel ge1, TestCollectionItemModel ge2)
    {
        return !(ge1 == ge2);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^ TestProperty.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return IsEqual(obj);
    }

    public bool Equals([AllowNull] TestCollectionItemModel obj)
    {
        return IsEqual(obj);
    }

    private bool IsEqual(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var ge2 = (TestCollectionItemModel)obj;
        return Id == ge2.Id && TestProperty == ge2.TestProperty;
    }
}
