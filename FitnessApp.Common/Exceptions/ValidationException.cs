using System;
using System.Diagnostics.CodeAnalysis;
using FitnessApp.Common.Abstractions.Models;

namespace FitnessApp.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException(ValidationError error) : Exception(error.ToString());
