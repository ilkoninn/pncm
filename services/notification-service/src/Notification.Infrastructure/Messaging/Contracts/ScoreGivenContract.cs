namespace Notification.Infrastructure.Messaging.Contracts;

public class ScoreGivenContract
{
    public Guid ContestEntryId { get; set; }
    public Guid GivenByUserId { get; set; }
    public DateTime OccurredAt { get; set; }
}
