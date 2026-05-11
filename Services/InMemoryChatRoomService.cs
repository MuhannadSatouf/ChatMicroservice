using ChatService.Api.Contracts;
using System.Collections.Concurrent;

namespace ChatService.Api.Services;

public sealed class InMemoryChatRoomService : IChatRoomService
{
    private readonly ConcurrentDictionary<string, ChatRoomResponse> _rooms = new();

    public ChatRoomResponse CreateRoom(CreateRoomRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Room name is required.");
        }

        var room = new ChatRoomResponse
        {
            RoomId = Guid.NewGuid().ToString("N"),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };

        _rooms[room.RoomId] = room;

        return room;
    }

    public IReadOnlyCollection<ChatRoomResponse> GetRooms()
    {
        return _rooms.Values
            .OrderByDescending(room => room.CreatedAt)
            .ToList();
    }

    public ChatRoomResponse? GetRoomById(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return null;
        }

        _rooms.TryGetValue(roomId.Trim(), out var room);

        return room;
    }
}