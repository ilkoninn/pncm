public interface IMediaGrpcClient
{
    Task<string?> GetAvatarUrlAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<string?> GetBannerUrlAsync(
        Guid userId,
        Guid bannerMediaId,
        CancellationToken cancellationToken = default);
}
