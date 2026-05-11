using ChatService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/chat/rooms/{roomId}/messages")]
public sealed class ChatMessagesController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;
    private readonly IChatMessageService _chatMessageService;

    public ChatMessagesController(
        IChatRoomService chatRoomService,
        IChatMessageService chatMessageService)
    {
        _chatRoomService = chatRoomService;
        _chatMessageService = chatMessageService;
    }

    [HttpGet]
    public IActionResult GetMessagesByRoomId(string roomId)
    {
        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
        {
            return NotFound(new
            {
                Error = "Room was not found."
            });
        }

        var messages = _chatMessageService.GetMessagesByRoomId(roomId);

        return Ok(messages);
    }
}