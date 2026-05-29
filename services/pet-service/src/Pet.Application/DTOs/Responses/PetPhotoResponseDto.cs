public sealed record PetPhotoResponseDto(
    Guid Id,
    Guid MediaId,
    bool IsPrimary,
    string? Url = null
);