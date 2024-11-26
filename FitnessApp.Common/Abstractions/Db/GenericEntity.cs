using MongoDB.Bson.Serialization.Attributes;

namespace FitnessApp.Common.Abstractions.Db;
public interface IGenericEntity
{
    [BsonId]
    string Id { get; init; }
}

public interface IUserId
{
    string UserId { get; init; }
}

public interface IWithUserIdEntity : IUserId, IGenericEntity;

public interface IPartitionKey
{
    string PartitionKey { get; init; }
}

public abstract class GenericEntity : IGenericEntity
{
    [BsonId]
    public string Id { get; init; }
}

public abstract class GenericWithUserIdEntity : GenericEntity, IWithUserIdEntity
{
    public string UserId { get; init; }
}

public interface IMultipleParamFilter;
