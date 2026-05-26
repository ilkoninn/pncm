public sealed record GetNotificationsByUserQuery(Guid UserId) : IRequest<IEnumerable<NotificationResponseDto>>;
