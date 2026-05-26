using Notification.Infrastructure.Messaging.Contracts;

public sealed class AdoptionApprovedConsumer(ISender sender) : IConsumer<AdoptionApprovedContract>
{
    public async Task Consume(ConsumeContext<AdoptionApprovedContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.AdopterId,
            "Müraciətiniz qəbul edildi!",
            "Adoption müraciətiniz sahibi tərəfindən qəbul edildi.",
            ENotificationType.AdoptionApproved
        ));
    }
}
