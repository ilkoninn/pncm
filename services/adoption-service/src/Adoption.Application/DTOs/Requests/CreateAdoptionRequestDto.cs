public sealed record CreateAdoptionRequestDto(
    Guid PetId,
    string Message,
    string ContactPhone
);
