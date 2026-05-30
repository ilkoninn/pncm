public sealed record CreatePostCommand(
    Guid UserId,
    Guid? PetId,
    string Content,
    List<Guid> MediaIds,
    string AuthorName,
    string? AuthorAvatarUrl
) : IRequest<PostResponseDto>;
