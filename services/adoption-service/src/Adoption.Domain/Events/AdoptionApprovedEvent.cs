namespace Adoption.Domain.Events;

public class AdoptionApprovedEvent
{
    public Guid AdoptionId { get; set; }
    public Guid AdopterId { get; set; }
}
