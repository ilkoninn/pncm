public interface IContestEntryRepository
{
    Task<ContestEntry> CreateAsync(ContestEntry entry, CancellationToken cancellationToken = default);
    Task<ContestEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ContestEntryResponseDto> AddScoreAsync(Guid entryId, Guid givenByUserId, CancellationToken ct = default);
}
