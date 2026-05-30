public sealed record CreateAdoptionRequestDto(
    Guid PetId,
    string Message,
    string ContactPhone,
    string PetName,
    string PetSlug,
    string? PetPrimaryPhotoUrl,
    Guid? PetPrimaryPhotoMediaId
);
