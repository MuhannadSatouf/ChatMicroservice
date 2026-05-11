using ChatService.Api.Contracts;

namespace ChatService.Api.Services;

public interface IOnlineUserService
{
    void AddUser(string connectionId, string roomId, string senderId, string senderName);
    OnlineUserResponse? RemoveUser(string connectionId, string roomId);
    OnlineUserResponse? RemoveConnection(string connectionId);
    IReadOnlyCollection<OnlineUserResponse> GetUsersByRoomId(string roomId);
    bool IsUserInRoom(string connectionId, string roomId);
}