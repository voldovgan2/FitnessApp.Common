using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.ServiceBus.Nats.Events;

[ExcludeFromCodeCoverage]
public class FollowRequestConfirmed
{
    public string UserId { get; set; }
    public string FollowerUserId { get; set; }
}