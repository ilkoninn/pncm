public sealed record GetContestByIdQuery(Guid Id) : IRequest<ContestResponseDto>;
