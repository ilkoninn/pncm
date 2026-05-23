public sealed class RequestAccessCommandHandler(
    IUserRepository userRepository,
    IMagicLinkService magicLinkService
) : IRequestHandler<RequestAccessCommand, RequestAccessResponseDto>
{
    public async Task<RequestAccessResponseDto> Handle(
        RequestAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        var isNewUser = user is null;

        if (request.Client == EClient.Web)
        {
            var token = await magicLinkService.GenerateMagicLinkTokenAsync(request.Email);
            Console.WriteLine($"[Magic Link] https://pncm.az/auth/magic?token={token}");
        }
        else
        {
            var otp = await magicLinkService.GenerateOtpAsync(request.Email);
            Console.WriteLine($"[OTP] {request.Email} → {otp}");
        }

        return new RequestAccessResponseDto(isNewUser);
    }
}
