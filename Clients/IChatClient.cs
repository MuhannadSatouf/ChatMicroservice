using ChatService.Api.Contracts;

namespace ChatService.Api.Clients;

public interface IChatClient
{
    Task Connected(object connectionInfo);

    Task UserJoined(object userInfo);

    Task UserLeft(object userInfo);

    Task ReceiveMessage(ChatMessageResponse message);

    Task Error(string message);
}