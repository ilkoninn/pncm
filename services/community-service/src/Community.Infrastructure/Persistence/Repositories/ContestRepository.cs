public sealed class ContestRepository(CommunityDbContext context) : IContestRepository
{
    public async Task<Contest> CreateAsync(Contest contest, CancellationToken cancellationToken = default)
    {
        await context.Contests.AddAsync(contest, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return contest;
    }

    public async Task<Contest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Contests.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<Contest>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Contests
            .Where(c => c.IsActive && !c.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<Contest> UpdateAsync(Contest contest, CancellationToken cancellationToken = default)
    {
        context.Contests.Update(contest);
        await context.SaveChangesAsync(cancellationToken);
        return contest;
    }

    public async Task<IEnumerable<ContestEntry>> GetLeaderboardAsync(Guid contestId, CancellationToken cancellationToken = default)
        => await context.ContestEntries
            .Where(e => e.ContestId == contestId)
            .OrderByDescending(e => e.Score)
            .ToListAsync(cancellationToken);
}
