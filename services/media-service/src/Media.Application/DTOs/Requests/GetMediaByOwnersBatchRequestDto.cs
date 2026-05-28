public sealed class GetMediaByOwnersBatchRequestDto
{
    public IEnumerable<Guid> OwnerIds { get; set; } = [];
    public EOwnerType OwnerType { get; set; }
}
