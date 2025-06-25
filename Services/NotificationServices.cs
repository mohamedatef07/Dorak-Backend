using Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class NotificationServices
    {
        private readonly ConcurrentDictionary<string, string> _userConnections = new();
        private readonly IHubContext<NotificationHub> hubContext;
        public NotificationServices(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        

        // Add user connection to the dictionary
        public void AddUserConnection(string userId, string connectionId)
        {
            _userConnections[userId] = connectionId;
        }

        // Remove user connection from the dictionary
        public async Task RemoveConnectedUser(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        // Get the connection ID for a given user
        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            var connectionId = GetConnectionId(userId);
            if (!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
        }
    }
}
