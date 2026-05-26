public abstract class BaseEvent
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
