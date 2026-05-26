public sealed record CreateNotificationCommand(
    Guid UserId,
    string Title,
    string Body,
    ENotificationType Type
) : IRequest<NotificationResponseDto>;
