using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FitnessApp.Common.Abstractions.Models;
using FitnessApp.Common.Exceptions;

namespace FitnessApp.Common.Helpers;

[ExcludeFromCodeCoverage]
public static class ValidationHelper
{
    public static ValidationError ValidateEmptyStringField(string fieldName, string value)
    {
        ValidationError result = null;
        if (string.IsNullOrWhiteSpace(value))
            result = new ValidationError($"{fieldName} can't be empty", fieldName);

        return result;
    }

    public static ValidationError ValidateEmptyIdsField(string fieldName, string[] value)
    {
        ValidationError result = null;
        if (value.Length == 0)
            result = new ValidationError($"{fieldName} can't be empty", fieldName);

        return result;
    }

    public static void ThrowExceptionIfNotValidatedEmptyStringField(string fieldName, string value)
    {
        var error = ValidateEmptyStringField(fieldName, value);
        if (error != null)
            throw new ValidationException(error);
    }

    public static void ThrowExceptionIfNotValidatedEmptyIdsField(string fieldName, string[] value)
    {
        var error = ValidateEmptyIdsField(fieldName, value);
        if (error != null)
            throw new ValidationException(error);
    }

    public static void ThrowExceptionIfNotValidFiles(FileImageModel[] images)
    {
        var errors = images.Select(image =>
        {
            FileValidationException exception = null;
            var fileFieldNameError = ValidateEmptyStringField(nameof(image.FieldName), image.FieldName);
            var fileFieldValueError = ValidateEmptyStringField(nameof(image.Value), image.Value);
            if (fileFieldNameError != null || fileFieldValueError != null)
                exception = new FileValidationException(fileFieldNameError, fileFieldValueError);

            return exception;
        }).Where(e => e != null);

        if (errors.Any())
            throw new AggregateException(errors);
    }

    public static ValidationError ValidateRange<T>(
        T min,
        T max,
        T value,
        string fieldName)
        where T : IComparable<T>
    {
        ValidationError result = null;
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            result = new ValidationError($"{fieldName} should be within the range [{min}, {max}]", fieldName);

        return result;
    }
}
