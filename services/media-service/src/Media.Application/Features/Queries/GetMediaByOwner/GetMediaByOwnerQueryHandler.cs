public sealed class GetMediaByOwnerQueryHandler(
    IMediaRepository mediaRepository,
    IStorageService storageService
) : IRequestHandler<GetMediaByOwnerQuery, IEnumerable<MediaFileResponseDto>>
{
    public async Task<IEnumerable<MediaFileResponseDto>> Handle(
        GetMediaByOwnerQuery request, CancellationToken cancellationToken)
    {
        var files = await mediaRepository.GetByOwnerAsync(
            request.OwnerId, request.OwnerType, cancellationToken);

        var result = new List<MediaFileResponseDto>();
        foreach (var f in files)
        {
            var url = await storageService.GetPresignedUrlAsync(
                f.ObjectKey, f.BucketName, cancellationToken: cancellationToken);
            result.Add(new MediaFileResponseDto(
                f.Id,
                f.OriginalFileName,
                url,
                f.ContentType,
                f.Size,
                f.OwnerType,
                f.OwnerId,
                f.CreatedAt
            ));
        }
        return result;
    }
}