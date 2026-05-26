public sealed class NotificationRepository(NotificationDbContext context) : INotificationRepository
{
    public async Task<NotificationDomain.Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IEnumerable<NotificationDomain.Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await context.Notifications
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .ToListAsync(ct);

    public async Task<NotificationDomain.Notification> CreateAsync(NotificationDomain.Notification notification, CancellationToken ct = default)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync(ct);
        return notification;
    }

    public async Task MarkAsReadAsync(Guid id, CancellationToken ct = default)
    {
        var notif = await context.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new KeyNotFoundException("Bildiriş tapılmadı.");
        notif.IsRead = true;
        await context.SaveChangesAsync(ct);
    }
}
