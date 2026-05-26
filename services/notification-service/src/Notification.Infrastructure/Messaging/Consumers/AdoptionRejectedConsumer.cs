using Notification.Infrastructure.Messaging.Contracts;

public sealed class AdoptionRejectedConsumer(ISender sender) : IConsumer<AdoptionRejectedContract>
{
    public async Task Consume(ConsumeContext<AdoptionRejectedContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.AdopterId,
            "Müraciətiniz rədd edildi.",
            "Adoption müraciətiniz rədd edildi.",
            ENotificationType.AdoptionRejected
        ));
    }
}
