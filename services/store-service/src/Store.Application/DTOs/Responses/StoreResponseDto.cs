public sealed record StoreResponseDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    decimal Latitude,
    decimal Longitude,
    string? Description,
    string? LogoUrl,
    string? PhoneNumber,
    bool IsActive,
    DateTime CreatedAt
);