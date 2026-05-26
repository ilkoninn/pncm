using Notification.Infrastructure.Messaging.Contracts;

public sealed class UserRegisteredConsumer(ISender sender) : IConsumer<UserRegisteredContract>
{
    public async Task Consume(ConsumeContext<UserRegisteredContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.UserId,
            "Xoş gəldiniz!",
            "Pəncəm ailəsinə xoş gəldiniz!",
            ENotificationType.UserRegistered
        ));
    }
}
