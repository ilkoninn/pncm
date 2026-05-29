public sealed record StoreResponseDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    decimal Latitude,
    decimal Longitude,
    string? Description,
    Guid? LogoMediaId,
    string? PhoneNumber,
    bool IsActive,
    DateTime CreatedAt,
    string? LogoUrl = null
);