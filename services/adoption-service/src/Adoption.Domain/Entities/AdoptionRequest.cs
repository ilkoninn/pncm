public class AdoptionRequest : AuditableEntity
{
    public Guid PetId { get; set; }
    public Guid AdopterId { get; set; }
    public EAdoptionStatus Status { get; set; } = EAdoptionStatus.Pending;
    public required string Message { get; set; }
    public required string ContactPhone { get; set; }
    public required string PetName { get; set; }
    public required string PetSlug { get; set; }
    public string? PetPrimaryPhotoUrl { get; set; }
    public Guid? PetPrimaryPhotoMediaId { get; set; }
    public required string AdopterName { get; set; }
    public Guid PetOwnerId { get; set; }
}
