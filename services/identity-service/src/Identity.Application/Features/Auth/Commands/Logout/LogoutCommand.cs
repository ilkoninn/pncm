public record LogoutCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<Unit>;