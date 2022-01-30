using FitnessApp.Common.Abstractions.Db.Configuration;
using FitnessApp.Common.Abstractions.Db.Entities.Base;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Db.DbContext
{
    public class DbContext<Entity>
        where Entity: IEntity
    {
        class SerliarizationService : CosmosSerializer
        {
            private readonly JsonConverter[] _converters;
            public SerliarizationService(JsonConverter[] converters)
            {
                _converters = converters;
            }

            public override T FromStream<T>(Stream stream)
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                stream.Close();
                var jsonData = Encoding.UTF8.GetString(bytes);
                var result = JsonConvert.DeserializeObject<T>(jsonData, _converters);
                return result;
            }

            public override Stream ToStream<T>(T input)
            {
                var jsonData = JsonConvert.SerializeObject(input, _converters);
                return new MemoryStream(Encoding.UTF8.GetBytes(jsonData));
            }
        }

        private readonly Container _container; 

        public DbContext(IOptions<CosmosDbSettings> settings, JsonConverter[] converters = null)
        {
            CosmosClientOptions cosmosClientOptions = converters != null ? 
                new CosmosClientOptions()
                {
                    Serializer = new SerliarizationService(converters),
                }
                : null;

            var cosmosClient = new CosmosClient(settings.Value.EndpointUri, settings.Value.PrimaryKey, cosmosClientOptions);
            var database = cosmosClient
                .CreateDatabaseIfNotExistsAsync(settings.Value.DatabaseId)
                .GetAwaiter()
                .GetResult()
                .Database;

            var containerProperties = new ContainerProperties 
            {
                Id = settings.Value.ContainerId,
                IndexingPolicy = new IndexingPolicy()
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent
                },
                PartitionKeyPath = settings.Value.PartitionKey
            };

            _container = database
                .CreateContainerIfNotExistsAsync(containerProperties)
                .GetAwaiter()
                .GetResult()
                .Container;
        }

        public Entity GetItemById(string id)
        {
            return GetItemAsListById(id).Single();
        }

        public Entity TryGetItemById(string id)
        {
            return GetItemAsListById(id).SingleOrDefault();
        }

        public IEnumerable<Entity> GetAllItems()
        {
            return _container
                    .GetItemLinqQueryable<Entity>(true)
                    .ToList();
        }

        public async Task<Entity> CreateItem(Entity entity)
        {
            return (await _container.CreateItemAsync(entity, new PartitionKey(entity.UserId))).Resource; 
        }

        public async Task<Entity> UpdateItem(Entity entity)
        {
            return (await _container.UpsertItemAsync(entity, new PartitionKey(entity.UserId))).Resource;
        }

        public async Task<bool> DeleteItem(string userId)
        {
            return (await _container.DeleteItemAsync<Entity>(userId, new PartitionKey(userId))).StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        private IEnumerable<Entity> GetItemAsListById(string id)
        {
            return _container
                    .GetItemLinqQueryable<Entity>(true)
                    .Where(i => i.UserId == id)
                    .ToList();
        }
    }    
}