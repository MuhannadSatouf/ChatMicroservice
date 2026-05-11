using ChatService.Api.Clients;
using ChatService.Api.Contracts;
using ChatService.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Hubs;

public sealed class ChatHub : Hub<IChatClient>
{
    private readonly IChatMessageService _chatMessageService;
    private readonly IChatRoomService _chatRoomService;
    private readonly ILogger<ChatHub> _logger;
    private readonly IOnlineUserService _onlineUserService;

    public ChatHub(
        IChatMessageService chatMessageService,
        IChatRoomService chatRoomService,
        IOnlineUserService onlineUserService,
        ILogger<ChatHub> logger)
    {
        _chatMessageService = chatMessageService;
        _chatRoomService = chatRoomService;
        _logger = logger;
        _onlineUserService = onlineUserService;
    }

    public async Task JoinRoom(string roomId, string senderId, string senderName)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        if (string.IsNullOrWhiteSpace(senderId))
            throw new HubException("SenderId is required.");

        if (string.IsNullOrWhiteSpace(senderName))
            throw new HubException("SenderName is required.");


        roomId = roomId.Trim();

        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
            throw new HubException("Room does not exist.");

        if (_onlineUserService.IsUserInRoom(Context.ConnectionId, roomId))
        {
            throw new HubException("You are already in this room.");
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        _onlineUserService.AddUser(
            Context.ConnectionId,
            roomId,
            senderId,
            senderName);

        var users = _onlineUserService.GetUsersByRoomId(roomId);

        await Clients.Group(roomId).UserJoined(new
        {
            RoomId = roomId,
            SenderId = senderId,
            SenderName = senderName,
            ConnectionId = Context.ConnectionId,
            JoinedAt = DateTimeOffset.UtcNow
        });

        await Clients.Group(roomId).OnlineUsersUpdated(users);
    }

    public async Task LeaveRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        roomId = roomId.Trim();

        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
            throw new HubException("Room does not exist.");

        var removedUser = _onlineUserService.RemoveUser(Context.ConnectionId, roomId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);

        var users = _onlineUserService.GetUsersByRoomId(roomId);

        await Clients.Group(roomId).UserLeft(new
        {
            RoomId = roomId,
            SenderId = removedUser?.SenderId,
            SenderName = removedUser?.SenderName,
            ConnectionId = Context.ConnectionId,
            LeftAt = DateTimeOffset.UtcNow
        });

        await Clients.Group(roomId).OnlineUsersUpdated(users);
    }

    public async Task SendMessage(SendMessageRequest request)
    {
        var room = _chatRoomService.GetRoomById(request.RoomId);

        if (room is null)
        {
            throw new HubException("Room does not exist.");
        }

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
        var removedUser = _onlineUserService.RemoveConnection(Context.ConnectionId);

        if (removedUser is not null)
        {
            var users = _onlineUserService.GetUsersByRoomId(removedUser.RoomId);

            await Clients.Group(removedUser.RoomId).UserLeft(new
            {
                RoomId = removedUser.RoomId,
                SenderId = removedUser.SenderId,
                SenderName = removedUser.SenderName,
                ConnectionId = Context.ConnectionId,
                LeftAt = DateTimeOffset.UtcNow
            });

            await Clients.Group(removedUser.RoomId).OnlineUsersUpdated(users);
        }

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

    public async Task Typing(string roomId, string senderName)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        if (string.IsNullOrWhiteSpace(senderName))
            throw new HubException("SenderName is required.");

        roomId = roomId.Trim();

        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
            throw new HubException("Room does not exist.");

        await Clients.OthersInGroup(roomId).UserTyping(new
        {
            RoomId = roomId,
            SenderName = senderName.Trim(),
            ConnectionId = Context.ConnectionId
        });
    }

    public async Task StopTyping(string roomId, string senderName)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            throw new HubException("RoomId is required.");

        if (string.IsNullOrWhiteSpace(senderName))
            throw new HubException("SenderName is required.");

        roomId = roomId.Trim();

        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
            throw new HubException("Room does not exist.");

        await Clients.OthersInGroup(roomId).UserStoppedTyping(new
        {
            RoomId = roomId,
            SenderName = senderName.Trim(),
            ConnectionId = Context.ConnectionId
        });
    }
}