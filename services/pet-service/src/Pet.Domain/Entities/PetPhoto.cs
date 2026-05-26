public class PetPhoto : BaseEntity
{
    public Guid PetId { get; set; }
    public Pet Pet { get; set; } = null!;
    public Guid MediaId { get; set; }
    public bool IsPrimary { get; set; }
}