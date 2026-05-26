public sealed record CreatePostRequestDto(
    Guid UserId,
    Guid? PetId,
    string Content,
    List<Guid> MediaIds
);
