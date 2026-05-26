public sealed class CreateContestEntryCommandHandler(IContestEntryRepository repository)
    : IRequestHandler<CreateContestEntryCommand, ContestEntryResponseDto>
{
    public async Task<ContestEntryResponseDto> Handle(CreateContestEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = new ContestEntry
        {
            ContestId = request.ContestId,
            PostId = request.PostId,
            UserId = request.UserId
        };

        await repository.CreateAsync(entry, cancellationToken);

        return entry.Adapt<ContestEntryResponseDto>();
    }
}
