public interface IContestRepository
{
    Task<Contest> CreateAsync(Contest contest, CancellationToken cancellationToken = default);
    Task<Contest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Contest> UpdateAsync(Contest contest, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContestEntry>> GetLeaderboardAsync(Guid contestId, CancellationToken cancellationToken = default);
}
