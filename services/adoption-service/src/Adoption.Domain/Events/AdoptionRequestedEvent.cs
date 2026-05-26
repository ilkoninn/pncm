namespace Adoption.Domain.Events;

public class AdoptionRequestedEvent
{
    public Guid AdoptionId { get; set; }
    public Guid PetId { get; set; }
    public Guid AdopterId { get; set; }
    public Guid OwnerId { get; set; }
}
