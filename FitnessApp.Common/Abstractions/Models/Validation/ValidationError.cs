using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Abstractions.Models.Validation;

[ExcludeFromCodeCoverage]
public class ValidationError(string message, string field)
{
    public override string ToString()
    {
        return $"Field validation failed, field name: {field}, message: {message}.";
    }
}
