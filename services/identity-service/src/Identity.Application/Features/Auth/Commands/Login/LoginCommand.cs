public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponseDto>;