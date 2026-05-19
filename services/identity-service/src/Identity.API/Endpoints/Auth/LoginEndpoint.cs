public class LoginEndpoint(IMediator mediator) 
    : Endpoint<LoginRequestDto, LoginResponseDto>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequestDto req, CancellationToken ct)
    {
        var command = new LoginCommand(
            req.Email, 
            req.Password);
        
        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}