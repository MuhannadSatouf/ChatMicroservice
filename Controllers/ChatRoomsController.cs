using ChatService.Api.Contracts;
using ChatService.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/chat/rooms")]
public sealed class ChatRoomsController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomsController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    [HttpPost]
    public IActionResult CreateRoom(CreateRoomRequest request)
    {
        try
        {
            var room = _chatRoomService.CreateRoom(request);

            return CreatedAtAction(
                nameof(GetRoomById),
                new { roomId = room.RoomId },
                room);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }

    [HttpGet]
    public IActionResult GetRooms()
    {
        var rooms = _chatRoomService.GetRooms();

        return Ok(rooms);
    }

    [HttpGet("{roomId}")]
    public IActionResult GetRoomById(string roomId)
    {
        var room = _chatRoomService.GetRoomById(roomId);

        if (room is null)
        {
            return NotFound(new
            {
                Error = "Room was not found."
            });
        }

        return Ok(room);
    }
}