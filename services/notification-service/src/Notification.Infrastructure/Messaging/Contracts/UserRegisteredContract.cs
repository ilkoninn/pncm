namespace Notification.Infrastructure.Messaging.Contracts;

public class UserRegisteredContract
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}
