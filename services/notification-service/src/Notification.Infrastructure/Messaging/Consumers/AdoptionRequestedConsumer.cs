using Notification.Infrastructure.Messaging.Contracts;

public sealed class AdoptionRequestedConsumer(ISender sender) : IConsumer<AdoptionRequestedContract>
{
    public async Task Consume(ConsumeContext<AdoptionRequestedContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.OwnerId,
            "Yeni müraciət!",
            "Heyvanınıza yeni adoption müraciəti var.",
            ENotificationType.AdoptionRequested
        ));
    }
}
