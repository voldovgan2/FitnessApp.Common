using MongoDB.Bson.Serialization.Attributes;

namespace FitnessApp.Common.Abstractions.Db;
public interface IGenericEntity
{
    [BsonId]
    string Id { get; set; }
}

public interface IUserId
{
    string UserId { get; set; }
}

public interface IWithUserIdEntity : IUserId, IGenericEntity;

public interface IPartitionKey
{
    string PartitionKey { get; set; }
}

public abstract class GenericEntity : IGenericEntity
{
    [BsonId]
    public string Id { get; set; }
}

public abstract class GenericWithUserIdEntity : GenericEntity, IWithUserIdEntity
{
    public string UserId { get; set; }
}

public interface IMultipleParamFilter;
