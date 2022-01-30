using Newtonsoft.Json;

namespace FitnessApp.Common.Abstractions.Db.Entities.Base
{
    public interface IEntity
    {
        [JsonProperty(PropertyName = "id")]
        string UserId { get; set; }
    }
}