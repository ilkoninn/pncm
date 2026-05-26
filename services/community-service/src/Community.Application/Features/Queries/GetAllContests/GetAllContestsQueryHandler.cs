public sealed class GetAllContestsQueryHandler(IContestRepository repository)
    : IRequestHandler<GetAllContestsQuery, IEnumerable<ContestResponseDto>>
{
    public async Task<IEnumerable<ContestResponseDto>> Handle(GetAllContestsQuery request, CancellationToken cancellationToken)
    {
        var contests = await repository.GetAllAsync(cancellationToken);

        return contests.Adapt<IEnumerable<ContestResponseDto>>();
    }
}
