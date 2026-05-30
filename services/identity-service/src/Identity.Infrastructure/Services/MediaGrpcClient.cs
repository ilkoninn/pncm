using Grpc.Net.Client;
using Media.API.Protos;

public sealed class MediaGrpcClient(IConfiguration configuration) : IMediaGrpcClient
{
    public async Task<string?> GetAvatarUrlAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:MediaService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new MediaGrpcService.MediaGrpcServiceClient(channel);

        var request = new GetPrimaryPhotosRequest { OwnerType = 0 };
        request.OwnerIds.Add(userId.ToString());

        var response = await client.GetPrimaryPhotosAsync(request, cancellationToken: cancellationToken);

        return response.Photos.FirstOrDefault()?.PhotoUrl;
    }

    public async Task<string?> GetBannerUrlAsync(
        Guid userId,
        Guid bannerMediaId,
        CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:MediaService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new MediaGrpcService.MediaGrpcServiceClient(channel);

        var response = await client.GetPhotosByOwnerAsync(
            new GetPhotosByOwnerRequest { OwnerId = userId.ToString(), OwnerType = 0 },
            cancellationToken: cancellationToken);

        return response.Photos
            .FirstOrDefault(p => p.MediaId == bannerMediaId.ToString())?.Url;
    }
}
