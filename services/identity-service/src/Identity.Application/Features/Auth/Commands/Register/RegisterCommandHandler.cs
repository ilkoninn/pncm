public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    UserManager<AppUser> userManager
) : IRequestHandler<RegisterCommand, RegisterResponseDto>
{
    public async Task<RegisterResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetByEmailAsync(request.Email);
        
        if (existingUser is not null)
            return new RegisterResponseDto("User already exists.");

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
            return new RegisterResponseDto("User registration failed.");

        return new RegisterResponseDto("User registered successfully.");
    }
}