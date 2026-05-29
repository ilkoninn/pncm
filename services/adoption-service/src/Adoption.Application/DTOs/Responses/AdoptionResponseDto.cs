public sealed record AdoptionResponseDto(
    Guid Id,
    Guid PetId,
    Guid AdopterId,
    Guid PetOwnerId,
    EAdoptionStatus Status,
    string Message,
    string ContactPhone,
    string PetName,
    string PetSlug,
    string? PetPrimaryPhotoUrl,
    string AdopterName,
    DateTime CreatedAt
);
