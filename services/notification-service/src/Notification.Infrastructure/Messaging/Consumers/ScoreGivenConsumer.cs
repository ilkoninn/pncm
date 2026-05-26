using Notification.Infrastructure.Messaging.Contracts;

public sealed class ScoreGivenConsumer(ISender sender) : IConsumer<ScoreGivenContract>
{
    public async Task Consume(ConsumeContext<ScoreGivenContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.GivenByUserId,
            "Skor aldınız!",
            "Yarışmada sizə yeni skor verildi.",
            ENotificationType.ScoreGiven
        ));
    }
}
