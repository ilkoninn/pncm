public class RegisterEndpoint(IMediator mediator) 
    : Endpoint<RegisterRequestDto, RegisterResponseDto>
{
    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequestDto req, CancellationToken ct)
    {
        var command = new RegisterCommand(
            req.FirstName,
            req.LastName,
            req.Email,
            req.PhoneNumber,
            req.Password,
            req.ConfirmPassword
        );

        var result = await mediator.Send(command, ct);
        await Send.OkAsync(result, ct);
    }
}