public class VerifyOtpEndpoint(IMediator mediator)
    : Endpoint<VerifyOtpRequestDto, VerifyOtpResponseDto>
{
    public override void Configure()
    {
        Post("/auth/otp/verify");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(VerifyOtpRequestDto req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isValid = await mediator.Send(
            new VerifyOtpCommand(userId, req.Code, req.Purpose), ct);

        await Send.OkAsync(new VerifyOtpResponseDto(isValid), ct);
    }
}