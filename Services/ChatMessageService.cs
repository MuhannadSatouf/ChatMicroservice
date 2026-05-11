using System.Collections.Concurrent;
using ChatService.Api.Contracts;
using ChatService.Api.Enums;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Services;

public sealed class ChatMessageService : IChatMessageService
{
    private readonly ConcurrentDictionary<string, List<ChatMessageResponse>> _messagesByRoom = new();

    public ChatMessageResponse CreateMessage(SendMessageRequest request)
    {
        ValidateMessage(request);

        var normalizedRoomId = request.RoomId.Trim();
        var normalizedClientMessageId = request.ClientMessageId.Trim();

        if (_messagesByRoom.TryGetValue(normalizedRoomId, out var existingMessages))
        {
            lock (existingMessages)
            {
                var existingMessage = existingMessages.FirstOrDefault(message =>
                    message.ClientMessageId == normalizedClientMessageId);

                if (existingMessage is not null)
                {
                    return existingMessage;
                }
            }
        }

        var message = new ChatMessageResponse
        {
            MessageId = Guid.NewGuid().ToString("N"),
            ClientMessageId = normalizedClientMessageId,
            RoomId = normalizedRoomId,
            SenderId = request.SenderId.Trim(),
            SenderName = request.SenderName.Trim(),
            Content = request.Content.Trim(),
            SentAt = DateTimeOffset.UtcNow,
            Type = request.Type,
            Status = ChatMessageStatus.Sent
        };

        var roomMessages = _messagesByRoom.GetOrAdd(message.RoomId, _ => new List<ChatMessageResponse>());

        lock (roomMessages)
        {
            roomMessages.Add(message);
        }

        return message;
    }

    public IReadOnlyCollection<ChatMessageResponse> GetMessagesByRoomId(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
        {
            return Array.Empty<ChatMessageResponse>();
        }

        if (!_messagesByRoom.TryGetValue(roomId.Trim(), out var messages))
        {
            return Array.Empty<ChatMessageResponse>();
        }

        lock (messages)
        {
            return messages
                .OrderBy(message => message.SentAt)
                .ToList();
        }
    }

    private static void ValidateMessage(SendMessageRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoomId))
            throw new HubException("RoomId is required.");

        if (string.IsNullOrWhiteSpace(request.SenderId))
            throw new HubException("SenderId is required.");

        if (string.IsNullOrWhiteSpace(request.SenderName))
            throw new HubException("SenderName is required.");

        if (string.IsNullOrWhiteSpace(request.Content))
            throw new HubException("Message content is required.");

        if (request.Content.Length > 1000)
            throw new HubException("Message content cannot be longer than 1000 characters.");
    }
}