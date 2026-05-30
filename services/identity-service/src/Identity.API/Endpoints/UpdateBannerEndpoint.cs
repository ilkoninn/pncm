public sealed class UpdateBannerEndpoint(IMediator mediator) : Endpoint<UpdateBannerEndpoint.Request>
{
    public record Request(Guid MediaId);

    public override void Configure()
    {
        Patch("/users/me/banner");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await mediator.Send(new UpdateBannerCommand(userId, req.MediaId), ct);
        await Send.NoContentAsync(ct);
    }
}
