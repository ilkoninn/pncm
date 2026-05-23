public sealed class RequestAccessEndpoint(IMediator mediator)
    : Endpoint<RequestAccessEndpoint.Request, RequestAccessResponseDto>
{
    public record Request(string Email, EClient Client);

    public override void Configure()
    {
        Post("/auth/request-access");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await mediator.Send(new RequestAccessCommand(req.Email, req.Client), ct);
        await Send.OkAsync(result, ct);
    }
}
