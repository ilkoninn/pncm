public sealed class DeleteMediaEndpoint(IMediator mediator) : Endpoint<EmptyRequest>
{
    public override void Configure()
    {
        Delete("/media/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        await mediator.Send(new DeleteMediaCommand(id), ct);
        await Send.NoContentAsync(ct);
    }
}