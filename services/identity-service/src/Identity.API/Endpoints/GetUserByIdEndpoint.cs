public sealed class GetUserByIdEndpoint(ISender sender) : EndpointWithoutRequest<UserPublicResponseDto>
{
    public override void Configure()
    {
        Get("/users/{id:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await sender.Send(new GetUserByIdQuery(id), ct);
        await Send.OkAsync(result, ct);
    }
}
