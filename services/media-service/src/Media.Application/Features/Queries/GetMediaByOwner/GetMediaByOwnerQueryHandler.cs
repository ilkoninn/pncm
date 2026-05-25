public sealed class GetMediaByOwnerQueryHandler(
    IMediaRepository mediaRepository
) : IRequestHandler<GetMediaByOwnerQuery, IEnumerable<MediaFileResponseDto>>
{
    public async Task<IEnumerable<MediaFileResponseDto>> Handle(
        GetMediaByOwnerQuery request, CancellationToken cancellationToken)
    {
        var files = await mediaRepository.GetByOwnerAsync(
            request.OwnerId, request.OwnerType, cancellationToken);

        return files.Select(f => new MediaFileResponseDto(
            f.Id,
            f.OriginalFileName,
            f.Url,
            f.ContentType,
            f.Size,
            f.OwnerType,
            f.OwnerId,
            f.CreatedAt
        ));
    }
}