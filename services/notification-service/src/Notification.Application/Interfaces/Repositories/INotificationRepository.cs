public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<Notification> CreateAsync(Notification notification, CancellationToken ct = default);
    Task MarkAsReadAsync(Guid id, CancellationToken ct = default);
}
