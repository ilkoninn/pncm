public sealed record CreateContestCommand(
    string Title,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string? Prize
) : IRequest<ContestResponseDto>;
