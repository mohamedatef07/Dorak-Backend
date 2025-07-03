using Dorak.DataTransferObject;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Repositories;
using System.Collections.Concurrent;

namespace Services
{
    public class NotificationServices
    {
        private readonly ConcurrentDictionary<string, AddUserToNotificationHubDTO> _userConnections = new();
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly NotificationRepository _notificationRepository;

        public NotificationServices(IHubContext<NotificationHub> hubContext, NotificationRepository notificationRepository)
        {
            _hubContext = hubContext;
            _notificationRepository = notificationRepository;
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

        public async Task SendMessage(string userId, NotificationDTO message)
        {

            var userConnectionId = _userConnections.Where(x => userId.Contains(x.Value.UserId)).Select(x => x.Key).FirstOrDefault();

            if (userConnectionId != null)
            {
                await _hubContext.Clients.Client(userConnectionId)
                .SendAsync("NewMessagePublished", message);
            }

        }
        public async Task SendMessageToMulitubleUsers(List<string> userIds, NotificationDTO message)
        {
            var userConnectionIds = _userConnections.Where(x => userIds.Contains(x.Value.UserId)).Select(x => x.Key).ToList();

            // Create the tasks to send the notification to all relevant users
            var tasks = userConnectionIds.Select(connection => _hubContext.Clients.Client(connection)
                .SendAsync("NewMessagePublished", message)).ToList();
            // Execute all tasks concurrently
            await Task.WhenAll(tasks);
        }

        public async Task RemoveConnectedUser(string userId)
        {
            _userConnections.TryRemove(userId, out _);
        }

        //// Get the connection ID for a given user
        //public string GetConnectionId(string userId)
        //{
        //    _userConnections.TryGetValue(userId, out var connectionId);
        //    return connectionId.ConnectionId;
        //}

        public PaginationApiResponse<List<NotificationDTO>> GetNotification(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var notification = _notificationRepository.Get(no => no.UserId == userId, pageSize, pageNumber).Select(no => new NotificationDTO
            {
                Title = no.Title,
                Message = no.Message,
                IsRead = no.IsRead,
                CreatedAt = no.CreatedAt,
            }).OrderByDescending(n => n.CreatedAt).ToList();

            var totalRecords = _notificationRepository.GetList(no => no.UserId == userId).Count();

            var paginationResponse = new PaginationApiResponse<List<NotificationDTO>>(
            success: true,
            message: "notifications retrived successfully.",
            status: 200,
            data: notification,
            totalRecords: totalRecords,
            currentPage: pageNumber,
            pageSize: pageSize);
            return paginationResponse;
        }
    }
}
