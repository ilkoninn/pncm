public sealed record CreateAdoptionCommand(
    Guid PetId,
    Guid AdopterId,
    string Message,
    string ContactPhone,
    string PetName,
    string PetSlug,
    string? PetPrimaryPhotoUrl,
    string AdopterName
) : IRequest<AdoptionResponseDto>;
