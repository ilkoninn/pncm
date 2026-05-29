public sealed record UpdateStoreCommand(
    Guid Id,
    Guid RequesterId,
    string Name,
    string Address,
    string City,
    decimal Latitude,
    decimal Longitude,
    string? Description,
    string? PhoneNumber,
    Guid? LogoMediaId,
    bool IsActive
) : IRequest<StoreResponseDto>;