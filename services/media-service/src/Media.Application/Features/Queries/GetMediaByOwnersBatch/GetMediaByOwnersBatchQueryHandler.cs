public sealed class GetMediaByOwnersBatchQueryHandler(
    IMediaRepository mediaRepository,
    IStorageService storageService
) : IRequestHandler<GetMediaByOwnersBatchQuery, Dictionary<Guid, IEnumerable<MediaFileResponseDto>>>
{
    public async Task<Dictionary<Guid, IEnumerable<MediaFileResponseDto>>> Handle(
        GetMediaByOwnersBatchQuery request, CancellationToken cancellationToken)
    {
        var files = await mediaRepository.GetByOwnersAsync(
            request.OwnerIds, request.OwnerType, cancellationToken);

        var result = new Dictionary<Guid, IEnumerable<MediaFileResponseDto>>();
        foreach (var group in files.GroupBy(f => f.OwnerId))
        {
            var dtos = new List<MediaFileResponseDto>();
            foreach (var f in group)
            {
                var url = await storageService.GetPresignedUrlAsync(
                    f.ObjectKey, f.BucketName, cancellationToken: cancellationToken);
                dtos.Add(new MediaFileResponseDto(
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
            result[group.Key] = dtos;
        }
        return result;
    }
}
