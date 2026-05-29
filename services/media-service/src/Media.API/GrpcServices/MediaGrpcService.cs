using Grpc.Core;
using Media.API.Protos;

namespace Media.API.GrpcServices;

public sealed class MediaGrpcService(IMediator mediator) : Protos.MediaGrpcService.MediaGrpcServiceBase
{
    public override async Task<GetPrimaryPhotosResponse> GetPrimaryPhotos(
        GetPrimaryPhotosRequest request, ServerCallContext context)
    {
        var ownerIds = request.OwnerIds
            .Select(id => Guid.Parse(id))
            .ToList();

        var ownerType = (EOwnerType)request.OwnerType;

        var mediaMap = await mediator.Send(
            new GetMediaByOwnersBatchQuery(ownerIds, ownerType),
            context.CancellationToken);

        var response = new GetPrimaryPhotosResponse();

        foreach (var (ownerId, files) in mediaMap)
        {
            var primary = files.FirstOrDefault();
            if (primary is null) continue;

            response.Photos.Add(new OwnerPhoto
            {
                OwnerId = ownerId.ToString(),
                PhotoUrl = primary.Url,
            });
        }

        return response;
    }

    public override async Task<GetPhotosByOwnerResponse> GetPhotosByOwner(
        GetPhotosByOwnerRequest request, ServerCallContext context)
    {
        var ownerId = Guid.Parse(request.OwnerId);
        var ownerType = (EOwnerType)request.OwnerType;

        var mediaMap = await mediator.Send(
            new GetMediaByOwnersBatchQuery([ownerId], ownerType),
            context.CancellationToken);

        var response = new GetPhotosByOwnerResponse();

        if (mediaMap.TryGetValue(ownerId, out var files))
        {
            foreach (var f in files)
            {
                response.Photos.Add(new PhotoItem
                {
                    MediaId = f.Id.ToString(),
                    Url = f.Url,
                });
            }
        }

        return response;
    }
}
