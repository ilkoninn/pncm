public sealed record CreateStoreCommand(
    string Name,
    string Address,
    string City,
    decimal Latitude,
    decimal Longitude,
    string? Description,
    Guid? LogoMediaId,
    string? PhoneNumber,
    Guid OwnerId
) : IRequest<StoreResponseDto>;