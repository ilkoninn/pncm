using Community.Infrastructure.Messaging.Contracts;

public sealed class AdoptionCompletedConsumer(ISender sender) : IConsumer<AdoptionCompletedContract>
{
    public async Task Consume(ConsumeContext<AdoptionCompletedContract> context)
    {
        var message = context.Message;
        var content = $"{message.NewOwnerName} artıq {message.PetName}-a sahib oldu! Pəncəm platforması üzərindən uğurlu övladlığa verilmə!";

        await sender.Send(new CreatePostCommand(
            message.NewOwnerId,
            message.PetId,
            content,
            []
        ));
    }
}
