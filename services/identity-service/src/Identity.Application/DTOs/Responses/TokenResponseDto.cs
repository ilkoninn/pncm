public sealed record TokenResponseDto(
    string? AccessToken,
    string? RefreshToken,
    DateTime? ExpiresAt,
    string Message
);
