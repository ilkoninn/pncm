public class Post : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid? PetId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<Guid> MediaIds { get; set; } = [];
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorAvatarUrl { get; set; }
}
