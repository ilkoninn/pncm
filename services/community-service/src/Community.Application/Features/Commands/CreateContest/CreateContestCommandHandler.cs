public sealed class CreateContestCommandHandler(IContestRepository repository)
    : IRequestHandler<CreateContestCommand, ContestResponseDto>
{
    public async Task<ContestResponseDto> Handle(CreateContestCommand request, CancellationToken cancellationToken)
    {
        var contest = new Contest
        {
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Prize = request.Prize
        };

        await repository.CreateAsync(contest, cancellationToken);

        return contest.Adapt<ContestResponseDto>();
    }
}
