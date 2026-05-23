public sealed record CreateStoreCommand(
    string Name,
    string Address,
    string City,
    decimal Latitude,
    decimal Longitude,
    string? Description,
    string? LogoUrl,
    string? PhoneNumber,
    Guid OwnerId
) : IRequest<StoreResponseDto>;