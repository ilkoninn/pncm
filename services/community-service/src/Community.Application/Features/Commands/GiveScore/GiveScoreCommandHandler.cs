public sealed class GiveScoreCommandHandler(IContestEntryRepository repository)
    : IRequestHandler<GiveScoreCommand, ContestEntryResponseDto>
{
    public async Task<ContestEntryResponseDto> Handle(GiveScoreCommand request, CancellationToken cancellationToken)
    {
        return await repository.AddScoreAsync(request.EntryId, request.GivenByUserId, cancellationToken);
    }
}
