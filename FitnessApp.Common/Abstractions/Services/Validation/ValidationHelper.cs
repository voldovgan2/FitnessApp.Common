using System;
using System.Collections.Generic;
using System.Linq;
using FitnessApp.Common.Abstractions.Models.BlobImage;
using FitnessApp.Common.Abstractions.Models.Validation;
using FitnessApp.Common.Exceptions;

namespace FitnessApp.Common.Abstractions.Services.Validation
{
    public static class ValidationHelper
    {
        public static ValidationError ValidateEmptyStringField(string fieldName, string value)
        {
            ValidationError result = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                result = new ValidationError($"{fieldName} can't be empty", fieldName);
            }

            return result;
        }

        public static void ThrowExceptionIfNotValidatedEmptyStringField(string fieldName, string value)
        {
            var error = ValidateEmptyStringField(fieldName, value);
            if (error != null)
                throw new ValidationException(error);
        }

        public static void ThrowExceptionIfNotValidBlobs(List<BlobImageModel> images)
        {
            var errors = images.Select(image =>
            {
                BlobValidationException exception = null;
                ValidationError blobFieldNameError = ValidateEmptyStringField(nameof(image.FieldName), image.FieldName);
                ValidationError blobFieldValueError = ValidateEmptyStringField(nameof(image.Value), image.Value);
                if (blobFieldNameError != null || blobFieldValueError != null)
                    exception = new BlobValidationException(blobFieldNameError, blobFieldValueError);

                return exception;
            }).Where(e => e != null);

            if (errors.Any())
                throw new AggregateException(errors);
        }

        public static ValidationError ValidateRange<T>(T min, T max, T value, string fieldName)
            where T : IComparable<T>
        {
            ValidationError result = null;
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                result = new ValidationError($"{fieldName} should be within the range [{min}, {max}]", fieldName);
            }

            return result;
        }
    }
}
