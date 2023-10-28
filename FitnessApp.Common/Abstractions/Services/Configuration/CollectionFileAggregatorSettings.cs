using System.Collections.Generic;

namespace FitnessApp.Common.Abstractions.Services.Configuration
{
    public class CollectionFileAggregatorSettings
    {
        public string ContainerName { get; set; }
        public Dictionary<string, string[]> CollectionsFileFields { get; set; }
    }
}
