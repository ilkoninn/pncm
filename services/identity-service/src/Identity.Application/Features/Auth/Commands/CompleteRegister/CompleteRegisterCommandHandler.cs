public sealed class CompleteRegisterCommandHandler(
    IMagicLinkService magicLinkService,
    UserManager<AppUser> userManager,
    IUserRepository userRepository,
    ITokenService tokenService
) : IRequestHandler<CompleteRegisterCommand, CompleteRegisterResponseDto>
{
    public async Task<CompleteRegisterResponseDto> Handle(
        CompleteRegisterCommand request, CancellationToken cancellationToken)
    {
        var email = await magicLinkService.ValidateRegistrationTokenAsync(request.RegistrationToken);

        if (email is null)
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure("registrationToken", "Qeydiyyat tokeni etibarsızdır və ya müddəti bitib.")
            ]);

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = email,
            UserName = email,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
            throw new ValidationException(
                result.Errors.Select(e => new FluentValidation.Results.ValidationFailure("user", e.Description)));

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        await userRepository.SaveRefreshTokenAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return new CompleteRegisterResponseDto(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(15));
    }
}
