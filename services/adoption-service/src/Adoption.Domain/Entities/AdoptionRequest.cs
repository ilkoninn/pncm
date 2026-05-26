public class AdoptionRequest : AuditableEntity
{
    public Guid PetId { get; set; }
    public Guid AdopterId { get; set; }
    public EAdoptionStatus Status { get; set; } = EAdoptionStatus.Pending;
    public required string Message { get; set; }
    public required string ContactPhone { get; set; }
}
