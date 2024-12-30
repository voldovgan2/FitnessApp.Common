using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models;

namespace FitnessApp.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class FileValidationException(ValidationError fileFieldNameError, ValidationError fileFieldValueError)
    : Exception(GetErrorMessage(fileFieldNameError, fileFieldValueError))
{
    private static string GetErrorMessage(ValidationError fileFieldNameError, ValidationError fileFieldValueError)
    {
        List<string> errors = [];
        if (fileFieldNameError != null)
            errors.Add(fileFieldNameError.ToString());

        if (fileFieldValueError != null)
            errors.Add(fileFieldValueError.ToString());

        return $"{string.Join(" and/or ", errors)}.";
    }
}
