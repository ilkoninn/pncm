public sealed record CreatePostCommand(
    Guid UserId,
    Guid? PetId,
    string Content,
    List<Guid> MediaIds
) : IRequest<PostResponseDto>;
