using ChatService.Api.Contracts;

namespace ChatService.Api.Services;

public interface IChatRoomService
{
    ChatRoomResponse CreateRoom(CreateRoomRequest request);
    IReadOnlyCollection<ChatRoomResponse> GetRooms();
    ChatRoomResponse? GetRoomById(string roomId);
}