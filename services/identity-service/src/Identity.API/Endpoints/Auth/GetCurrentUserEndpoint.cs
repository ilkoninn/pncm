public class GetCurrentUserEndpoint(IMediator mediator)
    : EndpointWithoutRequest<UserResponseDto>
{
    public override void Configure()
    {
        Get("/auth/me");
        Claims(ClaimTypes.NameIdentifier);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await mediator.Send(new GetCurrentUserQuery(userId), ct);
        await Send.OkAsync(result, ct);
    }
}