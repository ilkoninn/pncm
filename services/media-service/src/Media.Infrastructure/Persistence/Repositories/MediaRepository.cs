public sealed class MediaRepository(MediaDbContext context) : IMediaRepository
{
    public async Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.MediaFiles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<MediaFile>> GetByOwnerAsync(
        Guid ownerId, EOwnerType ownerType, CancellationToken cancellationToken = default)
        => await context.MediaFiles
            .Where(x => x.OwnerId == ownerId && x.OwnerType == ownerType)
            .ToListAsync(cancellationToken);

    public async Task<MediaFile> CreateAsync(MediaFile mediaFile, CancellationToken cancellationToken = default)
    {
        context.MediaFiles.Add(mediaFile);
        await context.SaveChangesAsync(cancellationToken);
        return mediaFile;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mediaFile = await context.MediaFiles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (mediaFile is null) return;
        context.MediaFiles.Remove(mediaFile);
        await context.SaveChangesAsync(cancellationToken);
    }
}