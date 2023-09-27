namespace FitnessApp.Common.Abstractions.Db.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollecttionName { get; set; }
    }
}