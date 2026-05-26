public sealed record ContestEntryResponseDto(
    Guid Id,
    Guid ContestId,
    Guid PostId,
    Guid UserId,
    int Score,
    DateTime CreatedAt
);
