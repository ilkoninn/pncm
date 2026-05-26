public sealed record ContestResponseDto(
    Guid Id,
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string? Prize,
    EContestStatus Status,
    DateTime CreatedAt
);
