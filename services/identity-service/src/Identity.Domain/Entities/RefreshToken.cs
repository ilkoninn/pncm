public class RefreshToken : AuditableEntity
{
    public required string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; } = false;

    // Foreign key to AppUser
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
