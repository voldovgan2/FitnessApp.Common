using System;

namespace FitnessApp.Common.Exceptions
{
    public class DuplicateCollectionItemException : Exception
    {
        public DuplicateCollectionItemException(string id)
            : base($"Item with id: {id} already exists")
        {
        }
    }
}
