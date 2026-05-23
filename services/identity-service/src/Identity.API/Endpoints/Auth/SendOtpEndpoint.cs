public class SendOtpEndpoint(IMediator mediator)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Post("/auth/otp/send");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var purpose = Query<EOtpPurpose>("purpose");

        await mediator.Send(new SendOtpCommand(userId, purpose), ct);
        await Send.NoContentAsync(ct);
    }
}