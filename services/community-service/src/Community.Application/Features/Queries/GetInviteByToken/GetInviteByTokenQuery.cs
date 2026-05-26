public sealed record GetInviteByTokenQuery(string Token) : IRequest<InviteResponseDto>;
