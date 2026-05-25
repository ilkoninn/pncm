public class MediaFile : AuditableEntity
{
    public required string FileName { get; set; }
    public required string OriginalFileName { get; set; }
    public required string ContentType { get; set; }
    public long Size { get; set; }
    public required string Url { get; set; }
    public required string BucketName { get; set; }
    public required string ObjectKey { get; set; }
    public Guid OwnerId { get; set; }
    public EOwnerType OwnerType { get; set; }
}