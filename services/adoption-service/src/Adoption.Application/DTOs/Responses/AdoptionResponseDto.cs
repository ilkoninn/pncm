public sealed record AdoptionResponseDto(
    Guid Id,
    Guid PetId,
    Guid AdopterId,
    EAdoptionStatus Status,
    string Message,
    string ContactPhone,
    DateTime CreatedAt
);
