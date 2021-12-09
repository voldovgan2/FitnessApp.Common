using FitnessApp.Common.Abstractions.Services.Configuration;
using FitnessApp.Common.Logger;
using FitnessApp.Common.Serializer.JsonSerializer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FitnessApp.Common.Abstractions.Services.Cache
{
    public class CacheService<Model> : ICacheService<Model>
    {
        private readonly CacheSettings _settings;
        private readonly IDistributedCache _distributedCache;
        private readonly IJsonSerializer _serializer;
        private readonly ILogger<CacheService<Model>> _log;

        public CacheService
        (
            IOptions<CacheSettings> settings, 
            IDistributedCache distributedCache,
            IJsonSerializer serializer,
            ILogger<CacheService<Model>> log
        )
        {
            _settings = settings.Value;
            _distributedCache = distributedCache;
            _serializer = serializer;
            _log = log;
        }

        public async Task<Model> GetItem(string id)
        {
            Model result = default;
            if (_settings.Enable)
            {
                var serialized = await _distributedCache.GetStringAsync(GetKey(id));
                if (serialized != null)
                {
                    result = _serializer.DeserializeFromString<Model>(serialized);
                    if (result == null)
                    {
                        _log.WriteWarning("Failed to deserialize data");
                    }
                }
                else
                {
                    _log.WriteWarning($"Failed to get item from cache. Item: {id}");
                }
            }
            return result;
        }

        public async Task SaveItem(string id, Model model)
        {
            if (_settings.Enable)
            {
                try
                {
                    await _distributedCache.SetStringAsync(GetKey(id), _serializer.SerializeToString(model));
                }
                catch (Exception ex)
                {
                    _log.WriteException(ex);
                }
            }
        }

        public async Task DeleteItem(string id)
        {
            if (_settings.Enable)
            {
                try
                {
                    await _distributedCache.RemoveAsync(GetKey(id));
                }
                catch (Exception ex)
                {
                    _log.WriteException(ex);
                }
            }
        }

        private string GetKey(string userId)
        {
            return $"{userId}_{_settings.Name}";
        }
    }
}