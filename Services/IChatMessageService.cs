using ChatService.Api.Contracts;

namespace ChatService.Api.Services;

public interface IChatMessageService
{
    CreateMessageResult CreateMessage(SendMessageRequest request);
    IReadOnlyCollection<ChatMessageResponse> GetMessagesByRoomId(string roomId);
}