using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models.Generic;

namespace FitnessApp.Comon.Tests.Shared.Abstraction.Models.Generic;

[ExcludeFromCodeCoverageAttribute]
public class TestGenericModel : IGenericModel
{
    public string UserId { get; set; }
    public string TestProperty1 { get; set; }
}