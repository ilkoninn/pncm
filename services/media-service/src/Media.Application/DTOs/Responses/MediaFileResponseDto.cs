public sealed record MediaFileResponseDto(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long Size,
    EOwnerType OwnerType,
    Guid OwnerId,
    DateTime CreatedAt
);