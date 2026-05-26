public sealed class GetNotificationsByUserQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetNotificationsByUserQuery, IEnumerable<NotificationResponseDto>>
{
    public async Task<IEnumerable<NotificationResponseDto>> Handle(GetNotificationsByUserQuery request, CancellationToken cancellationToken)
    {
        var notifications = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        return notifications.Adapt<IEnumerable<NotificationResponseDto>>();
    }
}
