public sealed record ToggleLikeCommand(Guid PostId, Guid UserId) : IRequest<ToggleLikeResponseDto>;
