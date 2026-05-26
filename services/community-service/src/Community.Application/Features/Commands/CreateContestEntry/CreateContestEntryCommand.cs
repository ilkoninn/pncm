public sealed record CreateContestEntryCommand(
    Guid ContestId,
    Guid PostId,
    Guid UserId
) : IRequest<ContestEntryResponseDto>;
