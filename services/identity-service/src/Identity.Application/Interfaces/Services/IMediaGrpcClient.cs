public interface IMediaGrpcClient
{
    Task<string?> GetAvatarUrlAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
