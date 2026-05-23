public sealed class SendOtpCommandHandler(
    IUserRepository userRepository,
    IOtpService otpService
) : IRequestHandler<SendOtpCommand, Unit>
{
    public async Task<Unit> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        if (user is null)
            throw new KeyNotFoundException("İstifadəçi tapılmadı.");

        var code = await otpService.GenerateOtpAsync(request.UserId, request.Purpose);

        // TODO: Send OTP via email or SMS based on the purpose
        Console.WriteLine($"OTP Code: {code}");

        return Unit.Value;
    }
}