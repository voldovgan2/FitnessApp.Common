namespace FitnessApp.Common.Abstractions.Db.Configuration
{
    public class CosmosDbSettings
    {
        public string EndpointUri { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
    }
}