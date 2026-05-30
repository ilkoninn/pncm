public sealed record PostResponseDto(
    Guid Id,
    Guid UserId,
    Guid? PetId,
    string Content,
    List<Guid> MediaIds,
    DateTime CreatedAt,
    string AuthorName,
    string? AuthorAvatarUrl = null,
    string? PrimaryPhotoUrl = null,
    List<string>? MediaUrls = null
);
