public sealed class VerifyAccessCommandHandler(
    IUserRepository userRepository,
    IMagicLinkService magicLinkService,
    ITokenService tokenService
) : IRequestHandler<VerifyAccessCommand, VerifyAccessResponseDto>
{
    public async Task<VerifyAccessResponseDto> Handle(
        VerifyAccessCommand request, CancellationToken cancellationToken)
    {
        var isValid = request.Client == EClient.Web
            ? await magicLinkService.ValidateMagicCodeAsync(request.Email, request.Code)
            : await magicLinkService.ValidateOtpAsync(request.Email, request.Code);

        if (!isValid)
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure("code", "Kod yanlışdır və ya müddəti bitib.")
            ]);

        var user = await userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            var registrationToken = await magicLinkService.StoreRegistrationTokenAsync(request.Email);
            return new VerifyAccessResponseDto(true, null, null, registrationToken, null);
        }

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await userRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new VerifyAccessResponseDto(false, accessToken, refreshToken, null, DateTime.UtcNow.AddMinutes(15));
    }
}
