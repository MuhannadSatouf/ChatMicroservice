namespace ChatService.Api.Contracts;

public sealed class CreateMessageResult
{
    public required ChatMessageResponse Message { get; init; }
    public required bool IsNew { get; init; }
}