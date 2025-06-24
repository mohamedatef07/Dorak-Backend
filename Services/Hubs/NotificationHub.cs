using Microsoft.AspNetCore.SignalR;
using Services;
using System.Security.Claims;

namespace Hubs
{
    public class NotificationHub : Hub
    {
        private static readonly Dictionary<string, string> _userConnections = new();
        private readonly NotificationServices notificationHubService;

        public NotificationHub(NotificationServices notificationHubService)
        {
            this.notificationHubService = notificationHubService;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                // Remove the connection from the service
                await notificationHubService.RemoveConnectedUser(userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId(string userId)
        {
            return notificationHubService.GetConnectionId(userId);
        }
    }
}
