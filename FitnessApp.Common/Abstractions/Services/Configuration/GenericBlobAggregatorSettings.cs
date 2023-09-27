namespace FitnessApp.Common.Abstractions.Services.Configuration
{
    public class GenericBlobAggregatorSettings
    {
        public string ContainerName { get; set; }
        public string[] BlobFields { get; set; }
    }
}