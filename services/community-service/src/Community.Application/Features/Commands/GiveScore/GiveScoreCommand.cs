public sealed record GiveScoreCommand(Guid EntryId, Guid GivenByUserId) : IRequest<ContestEntryResponseDto>;
