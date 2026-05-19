public class AppUser : IdentityUser<Guid>, IAuditable
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; } 
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public string? City { get; set; }

    // Additional properties for account management
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation property for VendorProfile
    public VendorProfile? VendorProfile { get; set; }
}