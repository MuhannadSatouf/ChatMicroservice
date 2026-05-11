using ChatService.Api.Enums;

namespace ChatService.Api.Contracts;

public sealed class ChatMessageResponse
{
    public required string MessageId { get; init; }
    public required string RoomId { get; init; }
    public required string SenderId { get; init; }
    public required string SenderName { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset SentAt { get; init; }
    public ChatMessageType Type { get; init; }
    public ChatMessageStatus Status { get; init; }
    public required string ClientMessageId { get; init; }
}