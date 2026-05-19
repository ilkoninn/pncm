public sealed record LoginResponseDto(
    string? AccessToken,
    string? RefreshToken,
    DateTime? ExpiresAt,
    string Message
);