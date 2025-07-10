using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Hubs
{
    public class QueueHub : Hub
    {
        private static readonly Dictionary<string, string> _userConnections = new();
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userConnections.Remove(userId);
            }
            return base.OnDisconnectedAsync(exception);
        }

        public static string GetConnectionId(string userId)
        {
            return _userConnections.TryGetValue(userId, out var connId) ? connId : null;
        }
        public async Task SendQueueStatusUpdate(int liveQueueId, string newStatus)
        {
            await Clients.All.SendAsync("ReceiveQueueStatusUpdate", liveQueueId, newStatus);
        }


        public async Task JoinShiftGroup(int shiftId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"shift_{shiftId}");
        }

        public async Task LeaveShiftGroup(int shiftId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"shift_{shiftId}");
        }
    }
}