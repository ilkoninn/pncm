using System.Collections.Concurrent;
using System.Threading.Channels;

public sealed class NotificationHub : INotificationHub
{
    private readonly ConcurrentDictionary<Guid, List<ChannelWriter<NotificationResponseDto>>> _clients = new();

    public Channel<NotificationResponseDto> Subscribe(Guid userId)
    {
        var channel = Channel.CreateUnbounded<NotificationResponseDto>();
        _clients.AddOrUpdate(
            userId,
            _ => [channel.Writer],
            (_, writers) => { lock (writers) { writers.Add(channel.Writer); } return writers; });
        return channel;
    }

    public void Unsubscribe(Guid userId, ChannelWriter<NotificationResponseDto> writer)
    {
        if (!_clients.TryGetValue(userId, out var writers)) return;
        lock (writers) { writers.Remove(writer); }
        writer.TryComplete();
    }

    public async Task SendAsync(Guid userId, NotificationResponseDto notification)
    {
        if (!_clients.TryGetValue(userId, out var writers)) return;
        List<ChannelWriter<NotificationResponseDto>> snapshot;
        lock (writers) { snapshot = [.. writers]; }
        foreach (var writer in snapshot)
            await writer.WriteAsync(notification);
    }
}
