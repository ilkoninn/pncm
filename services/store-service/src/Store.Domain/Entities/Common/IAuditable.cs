public interface IAuditable
{
    bool IsActive { get; set; }
    bool IsDeleted { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}