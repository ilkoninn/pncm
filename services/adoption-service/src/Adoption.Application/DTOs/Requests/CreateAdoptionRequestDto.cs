public sealed record CreateAdoptionRequestDto(
    Guid PetId,
    Guid AdopterId,
    string Message,
    string ContactPhone
);
