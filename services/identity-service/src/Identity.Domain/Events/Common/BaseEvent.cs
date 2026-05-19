public abstract class BaseEvent
{
    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
}