namespace FitnessApp.Common.ServiceBus.Nats.Events;

public class NewFollowRequest
{
    public string UserId { get; set; }
    public string UserToFollowId { get; set; }
}
