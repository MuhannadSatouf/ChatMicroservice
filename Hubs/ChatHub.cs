using ChatService.Api.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Hubs;

public sealed class ChatHub : Hub
{
  public async Task JoinRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", new
        {
            RoomId = roomId,
            ConnectionId = Context.ConnectionId,
            JoinedAt = DateTimeOffset.UtcNow
        });
    }

    public async Task LeaveRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        await Clients.Group(roomId).SendAsync("UserLeft", new
        {
            RoomId = roomId,
            ConnectionId = Context.ConnectionId,
            LeftAt = DateTimeOffset.UtcNow
        });
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoomId))
            throw new HubException("RoomId is required.");

        if (string.IsNullOrWhiteSpace(request.SenderId))
            throw new HubException("SenderId is required.");

        if (string.IsNullOrWhiteSpace(request.SenderName))
            throw new HubException("SenderName is required.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new HubException("Message content is required.");

        var message = new ChatMessageResponse
        {
            MessageId = Guid.NewGuid().ToString("N"),
            RoomId = request.RoomId,
            SenderId = request.SenderId,
            SenderName = request.SenderName,
            Content = request.Content.Trim(),
            SentAt = DateTimeOffset.UtcNow
        };

        await Clients.Group(request.RoomId).SendAsync("ReceiveMessage", message);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", new
        {
            ConnectionId = Context.ConnectionId,
            ConnectedAt = DateTimeOffset.UtcNow
        });

        await base.OnConnectedAsync();
    }
}