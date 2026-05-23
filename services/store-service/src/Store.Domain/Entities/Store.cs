public class PetStore : AuditableEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string City { get; set; }
    public required decimal Latitude { get; set; }
    public required decimal Longitude { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid OwnerId { get; set; }
}