using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/chat")]
public sealed class ChatInfoController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetInfo()
    {
        return Ok(new
        {
            Service = "Chat Microservice",
            Version = "1.0.0",
            RealtimeEndpoint = "/hubs/chat",
            HealthEndpoint = "/health",
            Features = new[]
            {
                "Join room",
                "Leave room",
                "Send message",
                "Receive live message"
            }
        });
    }
}