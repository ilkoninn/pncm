public sealed record CreateContestEntryRequestDto(
    Guid ContestId,
    Guid PostId,
    Guid UserId
);
