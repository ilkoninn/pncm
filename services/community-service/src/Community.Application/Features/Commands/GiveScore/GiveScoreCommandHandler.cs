public sealed class GiveScoreCommandHandler(
    IContestEntryRepository repository,
    ITopicProducer<ScoreGivenEvent> producer)
    : IRequestHandler<GiveScoreCommand, ContestEntryResponseDto>
{
    public async Task<ContestEntryResponseDto> Handle(GiveScoreCommand request, CancellationToken cancellationToken)
    {
        var result = await repository.AddScoreAsync(request.EntryId, request.GivenByUserId, cancellationToken);

        await producer.Produce(new ScoreGivenEvent
        {
            ContestEntryId = request.EntryId,
            GivenByUserId = request.GivenByUserId
        }, cancellationToken);

        return result;
    }
}
