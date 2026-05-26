namespace Notification.Infrastructure.Messaging.Contracts;

public class AdoptionApprovedContract
{
    public Guid AdoptionId { get; set; }
    public Guid AdopterId { get; set; }
}
