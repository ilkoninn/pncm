public sealed record CreateContestRequestDto(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string? Prize
);
