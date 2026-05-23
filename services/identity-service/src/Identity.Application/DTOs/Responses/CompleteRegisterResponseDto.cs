public sealed record CompleteRegisterResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
