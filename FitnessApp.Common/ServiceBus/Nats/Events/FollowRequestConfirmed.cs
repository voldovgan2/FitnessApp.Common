namespace FitnessApp.Common.ServiceBus.Nats.Events
{
    public class FollowRequestConfirmed
    {
        public string UserId { get; set; }
        public string FollowerUserId { get; set; }
    }
}