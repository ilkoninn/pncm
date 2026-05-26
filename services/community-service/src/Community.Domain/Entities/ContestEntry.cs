public class ContestEntry : AuditableEntity
{
    public Guid ContestId { get; set; }
    public Contest Contest { get; set; } = null!;
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public int Score { get; set; } = 0;
}
