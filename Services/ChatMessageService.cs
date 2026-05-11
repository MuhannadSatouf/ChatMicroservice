using ChatService.Api.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Services;

public sealed class ChatMessageService : IChatMessageService
{
    public ChatMessageResponse CreateMessage(SendMessageRequest request)
    {
        ValidateMessage(request);

        return new ChatMessageResponse
        {
            MessageId = Guid.NewGuid().ToString("N"),
            RoomId = request.RoomId.Trim(),
            SenderId = request.SenderId.Trim(),
            SenderName = request.SenderName.Trim(),
            Content = request.Content.Trim(),
            SentAt = DateTimeOffset.UtcNow
        };
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