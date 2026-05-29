using Notification.Infrastructure.Messaging.Contracts;

public sealed class AdoptionCompletedConsumer(ISender sender) : IConsumer<AdoptionCompletedContract>
{
    public async Task Consume(ConsumeContext<AdoptionCompletedContract> context)
    {
        var message = context.Message;
        await sender.Send(new CreateNotificationCommand(
            message.OriginalOwnerId,
            "Heyvan yeni sahibinə çatdı!",
            $"{message.PetName} artıq {message.NewOwnerName} tərəfindən götürüldü.",
            ENotificationType.AdoptionCompleted
        ));
    }
}
