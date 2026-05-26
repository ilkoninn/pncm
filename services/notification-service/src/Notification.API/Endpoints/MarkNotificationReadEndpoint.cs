public class MarkNotificationReadEndpoint(NotificationDbContext db) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Patch("/notifications/{id:guid}/read");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var notification = await db.Notifications.FindAsync(new object[] { id }, ct)
            ?? throw new KeyNotFoundException("Bildiriş tapılmadı.");
        notification.IsRead = true;
        await db.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}
