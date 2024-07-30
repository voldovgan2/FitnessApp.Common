using System.Diagnostics.CodeAnalysis;

namespace FitnessApp.Common.ServiceBus.Nats.Events;

[ExcludeFromCodeCoverage]
public class NewUserRegistered
{
    public string UserId { get; set; }
    public string Email { get; set; }
}
