public sealed class GetMediaByIdQueryHandler(
    IMediaRepository mediaRepository,
    IStorageService storageService
) : IRequestHandler<GetMediaByIdQuery, MediaFileResponseDto>
{
    public async Task<MediaFileResponseDto> Handle(
        GetMediaByIdQuery request, CancellationToken cancellationToken)
    {
        var mediaFile = await mediaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (mediaFile is null)
            throw new KeyNotFoundException("Media faylı tapılmadı.");

        var url = await storageService.GetPresignedUrlAsync(
            mediaFile.ObjectKey, mediaFile.BucketName, cancellationToken: cancellationToken);

        return new MediaFileResponseDto(
            mediaFile.Id,
            mediaFile.OriginalFileName,
            url,
            mediaFile.ContentType,
            mediaFile.Size,
            mediaFile.OwnerType,
            mediaFile.OwnerId,
            mediaFile.CreatedAt
        );
    }
}