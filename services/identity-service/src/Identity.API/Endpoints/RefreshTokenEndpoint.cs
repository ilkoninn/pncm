public sealed class RefreshTokenEndpoint(IMediator mediator)
    : Endpoint<RefreshTokenRequestDto, TokenResponseDto>
{
    public override void Configure()
    {
        Post("/auth/refresh-token");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshTokenRequestDto req, CancellationToken ct)
    {
        var command = new RefreshTokenCommand(req.AccessToken, req.RefreshToken);
        var result = await mediator.Send(command, ct);
        
        await Send.OkAsync(result, ct);
    }
}