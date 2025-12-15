using Microsoft.AspNetCore.SignalR;

namespace Riff.NotificationService.Hubs;

public class RiffHub(ILogger<RiffHub> logger) : Hub
{
    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        logger.LogInformation("Client {ConnectionId} joined room group {RoomId}", Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        logger.LogInformation("Client {ConnectionId} left room group {RoomId}", Context.ConnectionId, roomId);
    }
}