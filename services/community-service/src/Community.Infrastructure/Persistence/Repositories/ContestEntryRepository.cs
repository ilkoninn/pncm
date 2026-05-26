public sealed class ContestEntryRepository(CommunityDbContext context) : IContestEntryRepository
{
    public async Task<ContestEntry> CreateAsync(ContestEntry entry, CancellationToken cancellationToken = default)
    {
        await context.ContestEntries.AddAsync(entry, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<ContestEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.ContestEntries.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<ContestEntryResponseDto> AddScoreAsync(Guid entryId, Guid givenByUserId, CancellationToken ct = default)
    {
        var entry = await context.ContestEntries.FirstOrDefaultAsync(e => e.Id == entryId, ct)
            ?? throw new KeyNotFoundException("Yarışma iştirakçısı tapılmadı.");

        var alreadyScored = await context.ScoreEvents
            .AnyAsync(s => s.ContestEntryId == entryId && s.GivenByUserId == givenByUserId, ct);

        if (alreadyScored)
            throw new InvalidOperationException("Bu yarışmacıya artıq skor vermişsiniz.");

        var scoreEvent = new ScoreEvent
        {
            ContestEntryId = entryId,
            GivenByUserId = givenByUserId
        };

        await context.ScoreEvents.AddAsync(scoreEvent, ct);

        entry.Score += 1;
        context.ContestEntries.Update(entry);

        await context.SaveChangesAsync(ct);

        return entry.Adapt<ContestEntryResponseDto>();
    }
}
