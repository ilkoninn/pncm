public sealed class VerifyOtpCommandHandler(
    IOtpService otpService
) : IRequestHandler<VerifyOtpCommand, bool>
{
    public async Task<bool> Handle(
        VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        var isValid = await otpService.ValidateOtpAsync(
            request.UserId, request.Code, request.Purpose);

        return isValid;
    }
}