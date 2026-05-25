public sealed record UploadMediaCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long Size,
    Guid OwnerId,
    EOwnerType OwnerType
) : IRequest<MediaFileResponseDto>;