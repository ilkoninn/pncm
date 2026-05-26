public class GetNotificationsByUserEndpoint(ISender sender)
    : EndpointWithoutRequest<IEnumerable<NotificationResponseDto>>
{
    public override void Configure()
    {
        Get("/notifications/user/{userId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = Route<Guid>("userId");
        var result = await sender.Send(new GetNotificationsByUserQuery(userId), ct);
        await Send.OkAsync(result, ct);
    }
}
