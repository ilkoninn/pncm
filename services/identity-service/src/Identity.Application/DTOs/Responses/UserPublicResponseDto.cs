public record UserPublicResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    string? Bio,
    string? City
);
