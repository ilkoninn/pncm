public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService
) : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return new LoginResponseDto(null, null, null, "Token etibarsızdır.");

        var storedToken = await userRepository.GetRefreshTokenAsync(request.RefreshToken);

        if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            return new LoginResponseDto(null, null, null, "Refresh token etibarsızdır.");

        if (storedToken.UserId != Guid.Parse(userId))
            return new LoginResponseDto(null, null, null, "Token etibarsızdır.");

        await userRepository.RevokeRefreshTokenAsync(request.RefreshToken);

        var newAccessToken = tokenService.GenerateAccessToken(storedToken.User);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await userRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            Token = newRefreshToken,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new LoginResponseDto(newAccessToken, newRefreshToken, 
            DateTime.UtcNow.AddMinutes(15), "Token yeniləndi.");
    }
}