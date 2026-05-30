public sealed class DeleteAvatarEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/users/me/avatar");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await mediator.Send(new UpdateAvatarCommand(userId, null), ct);
        await Send.NoContentAsync(ct);
    }
}
