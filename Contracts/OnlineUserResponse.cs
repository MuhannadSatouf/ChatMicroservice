namespace ChatService.Api.Contracts;

public sealed class OnlineUserResponse
{
    public required string ConnectionId { get; init; }
    public required string RoomId { get; init; }
    public required string SenderId { get; init; }
    public required string SenderName { get; init; }
    public required DateTimeOffset JoinedAt { get; init; }
}