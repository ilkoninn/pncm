public class LogoutEndpoint(IMediator mediator)
    : Endpoint<LogoutRequestDto>
{
    public override void Configure()
    {
        Post("/auth/logout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LogoutRequestDto req, CancellationToken ct)
    {
        var command = new LogoutCommand(req.AccessToken, req.RefreshToken);
        await mediator.Send(command, ct);
        await Send.NoContentAsync(ct);
    }
}