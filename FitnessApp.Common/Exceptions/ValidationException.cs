using System;
using FitnessApp.Common.Abstractions.Models.Validation;

namespace FitnessApp.Common.Exceptions;

public class ValidationException(ValidationError error) : Exception(error.ToString());
