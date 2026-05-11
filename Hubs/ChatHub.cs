using ChatService.Api.Clients;
using ChatService.Api.Contracts;
using ChatService.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Hubs;

public sealed class ChatHub : Hub<IChatClient>
{
    private readonly IChatMessageService _chatMessageService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(
        IChatMessageService chatMessageService,
        ILogger<ChatHub> logger)
    {
        _chatMessageService = chatMessageService;
        _logger = logger;
    }

    public async Task JoinRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            throw new HubException("RoomId is required.");
        }

        roomId = roomId.Trim();

        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        _logger.LogInformation(
            "Connection {ConnectionId} joined room {RoomId}",
            Context.ConnectionId,
            roomId);

        await Clients.Group(roomId).UserJoined(new
        {
            RoomId = roomId,
            ConnectionId = Context.ConnectionId,
            JoinedAt = DateTimeOffset.UtcNow
        });
    }

    public async Task LeaveRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            throw new HubException("RoomId is required.");
        }

        roomId = roomId.Trim();

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        _logger.LogInformation(
            "Connection {ConnectionId} left room {RoomId}",
            Context.ConnectionId,
            roomId);

        await Clients.Group(roomId).UserLeft(new
        {
            RoomId = roomId,
            ConnectionId = Context.ConnectionId,
            LeftAt = DateTimeOffset.UtcNow
        });
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var message = _chatMessageService.CreateMessage(request);

        _logger.LogInformation(
            "Message {MessageId} sent to room {RoomId} by sender {SenderId}",
            message.MessageId,
            message.RoomId,
            message.SenderId);

        await Clients.Group(message.RoomId).ReceiveMessage(message);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected with connection id {ConnectionId}",
            Context.ConnectionId);

        await Clients.Caller.Connected(new
        {
            ConnectionId = Context.ConnectionId,
            ConnectedAt = DateTimeOffset.UtcNow
        });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (exception is not null)
        {
            _logger.LogWarning(
                exception,
                "Client disconnected with error. Connection id: {ConnectionId}",
                Context.ConnectionId);
        }
        else
        {
            _logger.LogInformation(
                "Client disconnected. Connection id: {ConnectionId}",
                Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}