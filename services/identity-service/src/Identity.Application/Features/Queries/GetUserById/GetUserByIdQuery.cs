public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserPublicResponseDto>;
