public sealed class UpdateAvatarEndpoint(IMediator mediator) : Endpoint<UpdateAvatarEndpoint.Request>
{
    public record Request(Guid MediaId);

    public override void Configure()
    {
        Patch("/users/me/avatar");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await mediator.Send(new UpdateAvatarCommand(userId, req.MediaId), ct);
        await Send.NoContentAsync(ct);
    }
}