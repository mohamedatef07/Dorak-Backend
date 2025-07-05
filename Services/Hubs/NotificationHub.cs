using Microsoft.AspNetCore.SignalR;
using Services;
using System.Security.Claims;

namespace Hubs
{
    public class NotificationHub : Hub
    {
        
        private readonly NotificationSignalRService _notificationSignalRService;

        public NotificationHub(NotificationSignalRService notificationSignalRService)
        {
            _notificationSignalRService = notificationSignalRService;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _notificationSignalRService.RemoveConnectedUser(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
     

    }
}
