public sealed class EndContestCommandHandler(
    IContestRepository repository,
    ITopicProducer<ContestEndedEvent> producer)
    : IRequestHandler<EndContestCommand, ContestResponseDto>
{
    public async Task<ContestResponseDto> Handle(EndContestCommand request, CancellationToken cancellationToken)
    {
        var contest = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Yarışma tapılmadı.");

        contest.Status = EContestStatus.Ended;

        var updated = await repository.UpdateAsync(contest, cancellationToken);

        await producer.Produce(new ContestEndedEvent
        {
            ContestId = updated.Id,
            Title = updated.Title
        }, cancellationToken);

        return updated.Adapt<ContestResponseDto>();
    }
}
