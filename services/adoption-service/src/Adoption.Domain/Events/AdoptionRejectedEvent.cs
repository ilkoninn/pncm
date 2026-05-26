namespace Adoption.Domain.Events;

public class AdoptionRejectedEvent
{
    public Guid AdoptionId { get; set; }
    public Guid AdopterId { get; set; }
}
