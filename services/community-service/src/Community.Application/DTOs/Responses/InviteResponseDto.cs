public sealed record InviteResponseDto(
    Guid Id,
    Guid ContestId,
    Guid InviterId,
    string Token,
    DateTime CreatedAt
);
