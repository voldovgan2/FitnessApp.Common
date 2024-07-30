using System;
using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.Exceptions;

[ExcludeFromCodeCoverage]
public class DuplicateCollectionItemException(string id) : Exception($"Item with id: {id} already exists");
