public class Pet : AuditableEntity
{
    public required string Name { get; set; }
    public ESpecies Species { get; set; }
    public string? Breed { get; set; }
    public int? AgeMonths { get; set; }
    public EGender Gender { get; set; }
    public EPetSize Size { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public bool IsVaccinated { get; set; }
    public bool IsNeutered { get; set; }
    public EPetStatus Status { get; set; } = EPetStatus.Available;
    public Guid OwnerId { get; set; }
    public EOwnerType OwnerType { get; set; }
    public string? OwnerFirstName { get; set; }
    public string? OwnerLastName { get; set; }
    public required string Slug { get; set; }
    public required string City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public ICollection<PetPhoto> Photos { get; set; } = [];
}