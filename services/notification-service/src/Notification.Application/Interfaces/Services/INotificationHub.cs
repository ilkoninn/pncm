using System.Threading.Channels;

public interface INotificationHub
{
    Channel<NotificationResponseDto> Subscribe(Guid userId);
    void Unsubscribe(Guid userId, ChannelWriter<NotificationResponseDto> writer);
    Task SendAsync(Guid userId, NotificationResponseDto notification);
}
