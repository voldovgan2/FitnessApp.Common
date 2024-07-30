using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.ServiceBus.Nats.Events;

[ExcludeFromCodeCoverage]
public class NewFollowRequest
{
    public string UserId { get; set; }
    public string UserToFollowId { get; set; }
}
