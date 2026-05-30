public sealed record CreatePostRequestDto(
    Guid? PetId,
    string Content,
    List<Guid> MediaIds,
    string? AuthorAvatarUrl = null
);
