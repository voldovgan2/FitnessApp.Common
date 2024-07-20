namespace FitnessApp.Common.ServiceBus.Nats.Events;

public class NewUserRegistered
{
    public string UserId { get; set; }
    public string Email { get; set; }
}
