using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Exceptions;

namespace FitnessApp.Common.Abstractions.Extensions;

[ExcludeFromCodeCoverage]
public static class ValidationErrorsListExtension
{
    public static void AddIfNotNull(this List<ValidationError> source, ValidationError item)
    {
        if (item != null)
            source.Add(item);
    }

    public static void ThrowIfNotEmpty(this List<ValidationError> source)
    {
        if (source.Count != 0)
            throw new AggregateException(source.Select(e => new ValidationException(e)));
    }
}