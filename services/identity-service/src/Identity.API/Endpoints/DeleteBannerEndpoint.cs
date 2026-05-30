public sealed class DeleteBannerEndpoint(IMediator mediator) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/users/me/banner");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await mediator.Send(new UpdateBannerCommand(userId, null), ct);
        await Send.NoContentAsync(ct);
    }
}
