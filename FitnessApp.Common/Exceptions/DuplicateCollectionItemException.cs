using System;

namespace FitnessApp.Common.Exceptions
{
    public class DuplicateCollectionItemException(string id) : Exception($"Item with id: {id} already exists");
}
