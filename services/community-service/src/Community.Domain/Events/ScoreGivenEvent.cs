public class ScoreGivenEvent : BaseEvent
{
    public Guid ContestEntryId { get; set; }
    public Guid GivenByUserId { get; set; }
}
