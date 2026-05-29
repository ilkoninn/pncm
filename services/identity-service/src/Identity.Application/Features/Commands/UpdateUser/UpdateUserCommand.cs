public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Bio,
    string? City
) : IRequest<UserResponseDto>;
