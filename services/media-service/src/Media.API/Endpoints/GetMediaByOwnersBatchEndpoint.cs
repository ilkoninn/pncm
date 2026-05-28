public sealed class GetMediaByOwnersBatchEndpoint(IMediator mediator)
    : Endpoint<GetMediaByOwnersBatchRequestDto, Dictionary<Guid, IEnumerable<MediaFileResponseDto>>>
{
    public override void Configure()
    {
        Post("/media/batch");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetMediaByOwnersBatchRequestDto req, CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetMediaByOwnersBatchQuery(req.OwnerIds, req.OwnerType), ct);
        await Send.OkAsync(result, ct);
    }
}
