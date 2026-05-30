using Grpc.Net.Client;
using Media.API.Protos;

public sealed class MediaGrpcClient(IConfiguration configuration) : IMediaGrpcClient
{
    public async Task<Dictionary<Guid, string>> GetPrimaryPhotosAsync(
        IEnumerable<Guid> ownerIds,
        int ownerType,
        CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:MediaService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new MediaGrpcService.MediaGrpcServiceClient(channel);

        var request = new GetPrimaryPhotosRequest { OwnerType = ownerType };
        request.OwnerIds.AddRange(ownerIds.Select(id => id.ToString()));

        var response = await client.GetPrimaryPhotosAsync(request, cancellationToken: cancellationToken);

        return response.Photos.ToDictionary(
            p => Guid.Parse(p.OwnerId),
            p => p.PhotoUrl);
    }

    public async Task<List<string>> GetPhotoUrlsByOwnerAsync(
        Guid ownerId,
        int ownerType,
        CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:MediaService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new MediaGrpcService.MediaGrpcServiceClient(channel);

        var request = new GetPhotosByOwnerRequest
        {
            OwnerId = ownerId.ToString(),
            OwnerType = ownerType,
        };

        var response = await client.GetPhotosByOwnerAsync(request, cancellationToken: cancellationToken);

        return response.Photos.Select(p => p.Url).ToList();
    }

    public async Task<List<(Guid MediaId, string Url)>> GetPhotoItemsByOwnerAsync(
        Guid ownerId,
        int ownerType,
        CancellationToken cancellationToken = default)
    {
        var address = configuration["GrpcServices:MediaService"]!;
        var channel = GrpcChannel.ForAddress(address);
        var client = new MediaGrpcService.MediaGrpcServiceClient(channel);

        var request = new GetPhotosByOwnerRequest
        {
            OwnerId = ownerId.ToString(),
            OwnerType = ownerType,
        };

        var response = await client.GetPhotosByOwnerAsync(request, cancellationToken: cancellationToken);
        return [.. response.Photos.Select(p => (Guid.Parse(p.MediaId), p.Url))];
    }
}
