namespace ChatService.Api.Contracts;

public sealed class ChatRoomResponse
{
    public required string RoomId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}