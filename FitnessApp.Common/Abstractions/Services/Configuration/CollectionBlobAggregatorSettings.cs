using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Services.Configuration
{
    public class CollectionBlobAggregatorSettings
    {
        public string ContainerName { get; set; }
        public Dictionary<string, string[]> CollectionsBlobFields { get; set; }
    }
}
