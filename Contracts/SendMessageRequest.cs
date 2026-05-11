namespace ChatService.Api.Contracts;

public sealed class SendMessageRequest
{
    public required string RoomId { get; init; }
    public required string SenderId { get; init; }
    public required string SenderName { get; init; }
    public required string Content { get; init; }
}