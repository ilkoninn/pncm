namespace Notification.Infrastructure.Messaging.Contracts;

public record AdoptionCompletedContract(
    Guid AdoptionId,
    Guid PetId,
    string PetName,
    string PetSlug,
    string? PetPrimaryPhotoUrl,
    Guid NewOwnerId,
    string NewOwnerName,
    Guid OriginalOwnerId);
