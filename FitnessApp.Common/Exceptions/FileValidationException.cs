using System;
using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Validation;

namespace FitnessApp.Common.Exceptions;

public class FileValidationException(ValidationError fileFieldNameError, ValidationError fileFieldValueError)
    : Exception(GetErrorMessage(fileFieldNameError, fileFieldValueError))
{
    private static string GetErrorMessage(ValidationError fileFieldNameError, ValidationError fileFieldValueError)
    {
        List<string> errors = new List<string>();
        if (fileFieldNameError != null)
            errors.Add(fileFieldNameError.ToString());

        if (fileFieldValueError != null)
            errors.Add(fileFieldValueError.ToString());

        return $"{string.Join(" and/or ", errors)}.";
    }
}
