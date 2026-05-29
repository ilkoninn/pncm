public interface IMediaGrpcClient
{
    Task<Dictionary<Guid, string>> GetPrimaryPhotosAsync(
        IEnumerable<Guid> ownerIds,
        int ownerType,
        CancellationToken cancellationToken = default);
}
