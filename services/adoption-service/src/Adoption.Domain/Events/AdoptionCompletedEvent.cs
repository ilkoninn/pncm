namespace Adoption.Domain.Events;

public record AdoptionCompletedEvent
{
    public Guid AdoptionId { get; init; }
    public Guid PetId { get; init; }
    public string PetName { get; init; } = string.Empty;
    public string PetSlug { get; init; } = string.Empty;
    public string? PetPrimaryPhotoUrl { get; init; }
    public Guid? PetPrimaryPhotoMediaId { get; init; }
    public Guid NewOwnerId { get; init; }
    public string NewOwnerName { get; init; } = string.Empty;
    public Guid OriginalOwnerId { get; init; }
}
