public sealed class GetContestByIdQueryHandler(IContestRepository repository)
    : IRequestHandler<GetContestByIdQuery, ContestResponseDto>
{
    public async Task<ContestResponseDto> Handle(GetContestByIdQuery request, CancellationToken cancellationToken)
    {
        var contest = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Yarışma tapılmadı.");

        return contest.Adapt<ContestResponseDto>();
    }
}
