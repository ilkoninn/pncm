public class OtpCode : AuditableEntity
{
    public required string Code { get; set; }
    public EOtpPurpose Purpose { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;

    // Foreign key to AppUser
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}