public sealed record GetLeaderboardQuery(Guid ContestId) : IRequest<IEnumerable<ContestEntryResponseDto>>;
