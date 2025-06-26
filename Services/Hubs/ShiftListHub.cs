using Dorak.DataTransferObject;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hubs
{
    public class ShiftListHub : Hub
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

    }
}
