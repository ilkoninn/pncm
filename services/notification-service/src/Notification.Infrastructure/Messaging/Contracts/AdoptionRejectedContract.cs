namespace Notification.Infrastructure.Messaging.Contracts;

public class AdoptionRejectedContract
{
    public Guid AdoptionId { get; set; }
    public Guid AdopterId { get; set; }
}
