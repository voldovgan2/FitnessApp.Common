using FitnessApp.Common.Abstractions.Db.Entities.Collection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FitnessApp.Common.Serializer.JsonConverters
{
    public class CollectionEntityConverter<Entity, CollectionItemEntity> : JsonConverter
        where Entity : ICollectionEntity
        where CollectionItemEntity : ICollectionItemEntity
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken.FromObject(value).WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var result = Activator.CreateInstance<Entity>();

            var userIdToken = jObject.SelectToken("id");
            result.UserId = userIdToken.ToObject<string>();

            var collectionToken = jObject.SelectToken("Collection");
            result.Collection = new Dictionary<string, List<ICollectionItemEntity>>();
            var collections = collectionToken.ToObject<Dictionary<string, List<CollectionItemEntity>>>();
            foreach (var kvp in collections)
            {
                var list = new List<ICollectionItemEntity>();
                foreach (var item in kvp.Value)
                {
                    list.Add(item);
                }
                result.Collection.Add(kvp.Key, list);
            }

            return result;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Entity);
        }
    }
}