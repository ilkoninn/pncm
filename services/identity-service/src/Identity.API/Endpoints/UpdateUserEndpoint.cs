public sealed class UpdateUserEndpoint(IMediator mediator) : Endpoint<UpdateUserEndpoint.Request, UserResponseDto>
{
    public record Request(string FirstName, string LastName, string? PhoneNumber, string? Bio, string? City);

    public override void Configure()
    {
        Patch("/users/me");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mediator.Send(new UpdateUserCommand(userId, req.FirstName, req.LastName, req.PhoneNumber, req.Bio, req.City), ct);
        await Send.OkAsync(result, ct);
    }
}
