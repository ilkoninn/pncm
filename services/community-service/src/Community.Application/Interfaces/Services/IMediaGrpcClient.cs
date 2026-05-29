public interface IMediaGrpcClient
{
    Task<Dictionary<Guid, string>> GetPrimaryPhotosAsync(
        IEnumerable<Guid> ownerIds,
        int ownerType,
        CancellationToken cancellationToken = default);

    Task<List<string>> GetPhotoUrlsByOwnerAsync(
        Guid ownerId,
        int ownerType,
        CancellationToken cancellationToken = default);
}
