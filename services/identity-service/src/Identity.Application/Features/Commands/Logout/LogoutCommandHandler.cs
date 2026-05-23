public sealed class LogoutCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    ITokenBlacklistService blacklistService
) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

        var expClaim = principal.FindFirstValue("exp");
        if (expClaim is not null)
        {
            var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim));
            var ttl = expTime - DateTimeOffset.UtcNow;

            if (ttl > TimeSpan.Zero)
                await blacklistService.BlacklistAsync(request.AccessToken, ttl);
        }

        await userRepository.RevokeRefreshTokenAsync(request.RefreshToken);

        return Unit.Value;
    }
}