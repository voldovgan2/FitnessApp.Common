using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

public class UpdateTestGenericModel : IUpdateGenericModel, IEquatable<UpdateTestGenericModel>
{
    public string UserId { get; set; }
    public string TestProperty1 { get; set; }

    public static bool operator ==(UpdateTestGenericModel ugm1, UpdateTestGenericModel ugm2)
    {
        if (ugm1 is null)
            return ugm2 is null;

        return ugm1.Equals(ugm2);
    }

    public static bool operator !=(UpdateTestGenericModel ugm1, UpdateTestGenericModel ugm2)
    {
        return !(ugm1 == ugm2);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode() ^ TestProperty1.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return IsEqual(obj);
    }

    public bool Equals([AllowNull] UpdateTestGenericModel obj)
    {
        return IsEqual(obj);
    }

    private bool IsEqual(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var ugm2 = (UpdateTestGenericModel)obj;
        return UserId == ugm2.UserId && TestProperty1 == ugm2.TestProperty1;
    }
}