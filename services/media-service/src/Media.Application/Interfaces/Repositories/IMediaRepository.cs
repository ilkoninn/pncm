public interface IMediaRepository
{
    Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MediaFile>> GetByOwnerAsync(Guid ownerId, EOwnerType ownerType, CancellationToken cancellationToken = default);
    Task<MediaFile> CreateAsync(MediaFile mediaFile, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}