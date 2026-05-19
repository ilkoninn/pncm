public class VendorProfile : AuditableEntity
{
    public required string StoreName { get; set; }
    public required string Address { get; set; }
    public required decimal Latitude { get; set; }
    public required decimal Longitude { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }

    // Foreign key to AppUser
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}