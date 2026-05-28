public sealed class UploadMediaCommandHandler(
    IStorageService storageService,
    IMediaRepository mediaRepository
) : IRequestHandler<UploadMediaCommand, MediaFileResponseDto>
{
    private const string BucketName = "pncm-media";

    public async Task<MediaFileResponseDto> Handle(
        UploadMediaCommand request, CancellationToken cancellationToken)
    {
        var objectKey = $"{request.OwnerType.ToString().ToLower()}/{request.OwnerId}/{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

        var url = await storageService.UploadAsync(
            request.FileStream,
            objectKey,
            request.ContentType,
            BucketName,
            cancellationToken);

        var mediaFile = new MediaFile
        {
            FileName = objectKey,
            OriginalFileName = request.FileName,
            ContentType = request.ContentType,
            Size = request.Size,
            Url = string.Empty,
            BucketName = BucketName,
            ObjectKey = objectKey,
            OwnerId = request.OwnerId,
            OwnerType = request.OwnerType
        };

        var created = await mediaRepository.CreateAsync(mediaFile, cancellationToken);

        var presignedUrl = await storageService.GetPresignedUrlAsync(
            created.ObjectKey, created.BucketName, cancellationToken: cancellationToken);

        return new MediaFileResponseDto(
            created.Id,
            created.OriginalFileName,
            presignedUrl,
            created.ContentType,
            created.Size,
            created.OwnerType,
            created.OwnerId,
            created.CreatedAt
        );
    }
}