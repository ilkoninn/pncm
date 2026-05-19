public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    UserManager<AppUser> userManager,
    ITokenService tokenService
) : IRequestHandler<LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        
        if (user is null)
            return new LoginResponseDto(null, null, null, "Invalid email or password.");

        var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
        
        if (!passwordValid)
            return new LoginResponseDto(null, null, null, "Invalid email or password.");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        return new LoginResponseDto(
            accessToken, 
            refreshToken, 
            DateTime.UtcNow.AddMinutes(15), 
            "Login successful."
        );
    }
}