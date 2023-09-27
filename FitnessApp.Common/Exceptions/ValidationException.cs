using System;
using FitnessApp.Common.Abstractions.Models.Validation;

namespace FitnessApp.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(ValidationError error)
            : base(error.ToString())
        {
        }
    }
}
