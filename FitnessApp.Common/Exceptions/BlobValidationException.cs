using System;
using System.Collections.Generic;
using FitnessApp.Common.Abstractions.Models.Validation;

namespace FitnessApp.Common.Exceptions
{
    public class BlobValidationException : Exception
    {
        public BlobValidationException(ValidationError blobFieldNameError, ValidationError blobFieldValueError)
            : base(GetErrorMessage(blobFieldNameError, blobFieldValueError))
        { }

        private static string GetErrorMessage(ValidationError blobFieldNameError, ValidationError blobFieldValueError)
        {
            List<string> errors = new List<string>();
            if (blobFieldNameError != null)
                errors.Add(blobFieldNameError.ToString());

            if (blobFieldValueError != null)
                errors.Add(blobFieldValueError.ToString());

            return $"{string.Join(" and/or ", errors)}.";
        }
    }
}
