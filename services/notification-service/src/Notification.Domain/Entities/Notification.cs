namespace NotificationDomain;

public class Notification : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public ENotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
}
