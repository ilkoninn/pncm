public sealed record EndContestCommand(Guid Id) : IRequest<ContestResponseDto>;
