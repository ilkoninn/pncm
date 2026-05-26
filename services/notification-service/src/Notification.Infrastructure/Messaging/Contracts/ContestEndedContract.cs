namespace Notification.Infrastructure.Messaging.Contracts;

public class ContestEndedContract
{
    public Guid ContestId { get; set; }
    public string Title { get; set; } = string.Empty;
}
