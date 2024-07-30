using System;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public class MultiWordSearchableAttribute : Attribute
{
}
