public sealed record VerifyAccessResponseDto(
    bool IsNewUser,
    string? AccessToken,
    string? RefreshToken,
    string? RegistrationToken,
    DateTime? ExpiresAt
);
