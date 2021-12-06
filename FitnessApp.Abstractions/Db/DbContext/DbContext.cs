using FitnessApp.Abstractions.Db.Configuration;
using FitnessApp.Abstractions.Db.Entities.Base;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace FitnessApp.Abstractions.Db.DbContext
{
    public class DbContext<Entity>
        where Entity: IEntity
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public DbContext(IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            _client = new MongoClient(_settings.ConnectionString);
            if (_client != null)
            {
                var mongoUrl = new MongoUrl(_settings.ConnectionString);
                _database = _client.GetDatabase(mongoUrl.DatabaseName);
            }
        }

        public IMongoCollection<Entity> Collection
        {
            get
            {
                return _database.GetCollection<Entity>(_settings.CollecttionName);
            }
        }

        public async Task<string> ExecuteTransacion(Func<Task> task)
        {
            string result = null;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == Environments.Development)
            {
                await Task.Run(task);
            }
            else
            {
                using (var session = await _client.StartSessionAsync())
                {
                    session.StartTransaction();
                    try
                    {
                        await Task.Run(task);
                        await session.CommitTransactionAsync();
                    }
                    catch (Exception e)
                    {
                        result = e.Message;
                        await session.AbortTransactionAsync();
                    }
                }
            }
            return result;
        }
    }
}