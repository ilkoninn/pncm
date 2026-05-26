public sealed class CreateNotificationCommandHandler(INotificationRepository repository)
    : IRequestHandler<CreateNotificationCommand, NotificationResponseDto>
{
    public async Task<NotificationResponseDto> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = request.UserId,
            Title = request.Title,
            Body = request.Body,
            Type = request.Type
        };

        await repository.CreateAsync(notification, cancellationToken);

        return notification.Adapt<NotificationResponseDto>();
    }
}
