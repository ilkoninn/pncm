public sealed record GetMediaByOwnerQuery(
    Guid OwnerId,
    EOwnerType OwnerType
) : IRequest<IEnumerable<MediaFileResponseDto>>;