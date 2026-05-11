using ChatService.Api.Contracts;
namespace ChatService.Api.Services;

public interface IChatMessageService
{
    ChatMessageResponse CreateMessage(SendMessageRequest request);
}