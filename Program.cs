using ChatService.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "AllowedClients";

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:") ||
                origin.StartsWith("https://localhost:") ||
                origin == "null"); // Allows file:// protocol
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors(CorsPolicyName);
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/", () => Results.Ok(new
{
    message = "ChatService API is running",
    endpoints = new
    {
        health = "/health",
        chat = "/hubs/chat"
    }
}));

app.Run();