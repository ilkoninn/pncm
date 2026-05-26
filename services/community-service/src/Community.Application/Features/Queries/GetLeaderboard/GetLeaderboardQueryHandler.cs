public sealed class GetLeaderboardQueryHandler(IContestRepository repository)
    : IRequestHandler<GetLeaderboardQuery, IEnumerable<ContestEntryResponseDto>>
{
    public async Task<IEnumerable<ContestEntryResponseDto>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var entries = await repository.GetLeaderboardAsync(request.ContestId, cancellationToken);

        return entries.Adapt<IEnumerable<ContestEntryResponseDto>>();
    }
}
