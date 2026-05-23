public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<TokenResponseDto>;