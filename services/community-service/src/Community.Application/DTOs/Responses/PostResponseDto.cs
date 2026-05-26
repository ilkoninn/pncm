public sealed record PostResponseDto(
    Guid Id,
    Guid UserId,
    Guid? PetId,
    string Content,
    List<Guid> MediaIds,
    DateTime CreatedAt
);
