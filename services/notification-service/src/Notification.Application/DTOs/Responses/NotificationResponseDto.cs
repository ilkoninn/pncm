public sealed record NotificationResponseDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Body,
    ENotificationType Type,
    bool IsRead,
    DateTime CreatedAt
);
