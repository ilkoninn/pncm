public sealed class VerifyAccessEndpoint(IMediator mediator)
    : Endpoint<VerifyAccessEndpoint.Request, VerifyAccessResponseDto>
{
    public record Request(string Email, string Code, EClient Client);

    public override void Configure()
    {
        Post("/auth/verify-access");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new VerifyAccessCommand(req.Email, req.Code, req.Client), ct);
        await Send.OkAsync(result, ct);
    }
}
