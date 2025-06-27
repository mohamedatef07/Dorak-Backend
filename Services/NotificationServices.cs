using Dorak.DataTransferObject;
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
        private readonly ConcurrentDictionary<string, AddUserToNotificationHubDTO> _userConnections = new();
        private readonly IHubContext<NotificationHub> hubContext;
        public NotificationServices(IHubContext<NotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }

        public async Task<bool> AddToUserNotificationHub(string userConnectionId, AddUserToNotificationHubDTO userInfo)
        {
            // Lock to prevent race conditions when accessing shared resources
            lock (_userConnections)
            {
                // Check if the connectionId already exists in the dictionary
                foreach (var connectedUser in _userConnections)
                {
                    if (connectedUser.Key.ToLower() == userConnectionId.ToLower())
                        return false; // Return false if the user is already in the dictionary
                }

                // Add the new user to the dictionary if they don't exist
                _userConnections.TryAdd(userConnectionId, userInfo);
            }

            // If the user is of type Agent, publish the notification to the agent (You can customize the condition)
            if (userInfo.Role.ToLower() == "Provider" && userInfo != null)
            {
                await NewAgentUserAddedPublisher(userInfo.UserId, userInfo); // Adjust this as needed
            }

            return true;
        }

        public string GetConnectionIdByUserId(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.Where(x => x.Value.UserId == userId).Select(x => x.Key).FirstOrDefault();
            }
        }

        public List<string> GetAllConnectionIdsForUser(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.Where(x => x.Value.UserId == userId).Select(x => x.Key).ToList();
            }
        }

        private async Task NewAgentUserAddedPublisher(string userId, AddUserToNotificationHubDTO userInfo)
        {
            var userConnectionIds = _userConnections.Where(x => userId.Contains(x.Value.UserId)).Select(x => x.Key).ToList();

            // Create the tasks to send the notification to all relevant users
            var tasks = userConnectionIds.Select(connection => hubContext.Clients.Client(connection)
                .SendAsync("NewOnlineUser", new
                {
                    UserId = userId,
                    Role = userInfo.Role,
                    Name = userInfo.Name

                })).ToList();

            // Execute all tasks concurrently
            await Task.WhenAll(tasks);
        }

        // Add user connection to the dictionary
        

        // Remove user connection from the dictionary
        public async Task RemoveConnectedUser(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        // Get the connection ID for a given user
        public string GetConnectionId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId.ConnectionId;
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
