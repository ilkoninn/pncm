public sealed record CreateInviteCommand(Guid ContestId, Guid InviterId) : IRequest<InviteResponseDto>;
