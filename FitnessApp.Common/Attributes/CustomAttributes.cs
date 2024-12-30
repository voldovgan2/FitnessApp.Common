using System;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Attributes;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public class MultiWordSearchableAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public class SensitiveAttribute : Attribute;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Property)]
public class SingleWordSearchableAttribute : Attribute;