public class GetNotificationsByUserEndpoint(ISender sender)
    : EndpointWithoutRequest<IEnumerable<NotificationResponseDto>>
{
    public override void Configure()
    {
        Get("/notifications/me");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }
        var result = await sender.Send(new GetNotificationsByUserQuery(userId), ct);
        await Send.OkAsync(result, ct);
    }
}
