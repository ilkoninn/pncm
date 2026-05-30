public record UserResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    Guid? AvatarMediaId,
    string? AvatarUrl = null,
    string? Bio = null,
    string? City = null,
    Guid? BannerMediaId = null,
    string? BannerUrl = null
);