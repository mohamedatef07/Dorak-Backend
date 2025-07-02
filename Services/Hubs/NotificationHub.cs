using Microsoft.AspNetCore.SignalR;
using Services;
using System.Security.Claims;

namespace Hubs
{
    public class NotificationHub : Hub
    {
        
        private readonly NotificationServices notificationService;

        public NotificationHub(NotificationServices notificationHubService)
        {
            this.notificationService = notificationHubService;
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await notificationService.RemoveConnectedUser(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        #region Old
        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        // Add the user connection to the service
        //        notificationService.AddUserConnection(userId, Context.ConnectionId);
        //        await Clients.Caller.SendAsync("UserConnected", Context.ConnectionId);
        //    }

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        // Remove the connection from the service
        //        await notificationService.RemoveConnectedUser(userId);
        //    }
        //    await base.OnDisconnectedAsync(exception);
        //} 
        //public string GetConnectionId(string userId)
        //{
        //    return notificationService.GetConnectionId(userId);
        //}
        //public async Task SendNotificationToUser(string userId, string message)
        //{
        //    await notificationService.SendNotificationToUser(userId, message);
        //}
        #endregion

    }
}
