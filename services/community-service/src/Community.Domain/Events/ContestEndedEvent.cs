namespace Community.Domain.Events;

public class ContestEndedEvent
{
    public Guid ContestId { get; set; }
    public string Title { get; set; } = string.Empty;
}
