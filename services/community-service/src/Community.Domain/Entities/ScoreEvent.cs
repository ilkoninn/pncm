public class ScoreEvent : BaseEntity
{
    public Guid ContestEntryId { get; set; }
    public Guid GivenByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
