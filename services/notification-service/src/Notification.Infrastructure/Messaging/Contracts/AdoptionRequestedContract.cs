namespace Notification.Infrastructure.Messaging.Contracts;

public class AdoptionRequestedContract
{
    public Guid AdoptionId { get; set; }
    public Guid PetId { get; set; }
    public Guid AdopterId { get; set; }
    public Guid OwnerId { get; set; }
}
