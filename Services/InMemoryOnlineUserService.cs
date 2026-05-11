using ChatService.Api.Contracts;
using System.Collections.Concurrent;

namespace ChatService.Api.Services;

public sealed class InMemoryOnlineUserService : IOnlineUserService
{
    private readonly ConcurrentDictionary<string, OnlineUserResponse> _usersByConnection = new();

    public void AddUser(string connectionId, string roomId, string senderId, string senderName)
    {
        var user = new OnlineUserResponse
        {
            ConnectionId = connectionId,
            RoomId = roomId.Trim(),
            SenderId = senderId.Trim(),
            SenderName = senderName.Trim(),
            JoinedAt = DateTimeOffset.UtcNow
        };

        _usersByConnection[connectionId] = user;
    }

    public OnlineUserResponse? RemoveUser(string connectionId, string roomId)
    {
        if (_usersByConnection.TryGetValue(connectionId, out var user)
            && user.RoomId == roomId.Trim())
        {
            _usersByConnection.TryRemove(connectionId, out var removedUser);
            return removedUser;
        }

        return null;
    }

    public OnlineUserResponse? RemoveConnection(string connectionId)
    {
        _usersByConnection.TryRemove(connectionId, out var removedUser);

        return removedUser;
    }

    public IReadOnlyCollection<OnlineUserResponse> GetUsersByRoomId(string roomId)
    {
        return _usersByConnection.Values
            .Where(user => user.RoomId == roomId.Trim())
            .OrderBy(user => user.SenderName)
            .ToList();
    }
    public bool IsUserInRoom(string connectionId, string roomId)
    {
        return _usersByConnection.TryGetValue(connectionId, out var user)
               && user.RoomId == roomId.Trim();
    }
}