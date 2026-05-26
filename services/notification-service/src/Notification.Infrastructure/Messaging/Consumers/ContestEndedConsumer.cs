using Notification.Infrastructure.Messaging.Contracts;

public sealed class ContestEndedConsumer(ISender sender) : IConsumer<ContestEndedContract>
{
    public async Task Consume(ConsumeContext<ContestEndedContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            Guid.Empty,
            "Yarışma bitdi!",
            $"'{message.Title}' yarışması başa çatdı.",
            ENotificationType.ContestEnded
        ));
    }
}
