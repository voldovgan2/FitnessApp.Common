using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.Entities.Base;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace FitnessApp.Common.Abstractions.Db.DbContext
{
    public class DbContext<Entity>
        where Entity: IEntity
    {
        private readonly CosmosClient _cosmosClient;  

        public DbContext(IOptions<CosmosDbSettings> settings)
        {
            _cosmosClient = new CosmosClient(settings.Value.EndpointUri, settings.Value.PrimaryKey);
            Container = _cosmosClient.GetContainer(settings.Value.DatabaseId, settings.Value.ContainerId);
        }

        public Container Container { get; private set; }
    }
}