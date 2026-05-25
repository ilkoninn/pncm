public sealed class GetMediaByIdEndpoint(IMediator mediator) : Endpoint<EmptyRequest, MediaFileResponseDto>
{
    public override void Configure()
    {
        Get("/media/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await mediator.Send(new GetMediaByIdQuery(id), ct);
        await Send.OkAsync(result, ct);
    }
}