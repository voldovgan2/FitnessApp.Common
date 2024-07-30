using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

[ExcludeFromCodeCoverageAttribute]
public class CreateTestGenericModel : ICreateGenericModel, IEquatable<CreateTestGenericModel>
{
    public string UserId { get; set; }
    public string TestProperty1 { get; set; }

    public static bool operator ==(CreateTestGenericModel cgm1, CreateTestGenericModel cgm2)
    {
        if (cgm1 is null)
            return cgm2 is null;

        return cgm1.Equals(cgm2);
    }

    public static bool operator !=(CreateTestGenericModel cgm1, CreateTestGenericModel cgm2)
    {
        return !(cgm1 == cgm2);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode() ^ TestProperty1.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return IsEqual(obj);
    }

    public bool Equals([AllowNull] CreateTestGenericModel obj)
    {
        return IsEqual(obj);
    }

    private bool IsEqual(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var cgm2 = (CreateTestGenericModel)obj;
        return UserId == cgm2.UserId && TestProperty1 == cgm2.TestProperty1;
    }
}
