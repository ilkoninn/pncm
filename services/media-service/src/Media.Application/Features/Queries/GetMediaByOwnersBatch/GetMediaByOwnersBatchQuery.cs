public sealed record GetMediaByOwnersBatchQuery(
    IEnumerable<Guid> OwnerIds,
    EOwnerType OwnerType
) : IRequest<Dictionary<Guid, IEnumerable<MediaFileResponseDto>>>;
