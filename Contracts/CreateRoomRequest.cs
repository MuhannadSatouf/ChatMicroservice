namespace ChatService.Api.Contracts;
public sealed class CreateRoomRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
}