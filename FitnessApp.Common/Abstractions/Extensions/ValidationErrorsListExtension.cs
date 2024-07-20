using System;
using System.Collections.Generic;
using System.Linq;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Exceptions;

namespace FitnessApp.Common.Abstractions.Extensions;

public static class ValidationErrorsListExtension
{
    public static void AddIfNotNull(this List<ValidationError> source, ValidationError item)
    {
        if (item != null)
            source.Add(item);
    }

    public static void ThrowIfNotEmpty(this List<ValidationError> source)
    {
        if (source.Any())
            throw new AggregateException(source.Select(e => new ValidationException(e)));
    }

    public static void ThrowIfNotEmpty(this IEnumerable<ValidationError> source)
    {
        if (source.Any())
            throw new AggregateException(source.Select(e => new ValidationException(e)));
    }
}