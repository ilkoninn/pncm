public sealed class CompleteRegisterEndpoint(IMediator mediator)
    : Endpoint<CompleteRegisterEndpoint.Request, CompleteRegisterResponseDto>
{
    public record Request(string RegistrationToken, string FirstName, string LastName, string? PhoneNumber);

    public override void Configure()
    {
        Post("/auth/complete-register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new CompleteRegisterCommand(req.RegistrationToken, req.FirstName, req.LastName, req.PhoneNumber), ct);
        await Send.CreatedAtAsync("/auth/me", null, result, cancellation: ct);
    }
}
