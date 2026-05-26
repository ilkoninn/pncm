public class Contest : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Prize { get; set; }
    public EContestStatus Status { get; set; } = EContestStatus.Draft;
    public List<ContestEntry> Entries { get; set; } = [];
}
