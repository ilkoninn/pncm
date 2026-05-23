public record UserResponseDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber
);