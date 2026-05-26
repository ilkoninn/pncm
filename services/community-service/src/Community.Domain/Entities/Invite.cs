public class Invite : BaseEntity
{
    public Guid ContestId { get; set; }
    public Guid InviterId { get; set; }
    public string Token { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
}
