using Dorak.DataTransferObject;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Repositories;
using System.Collections.Concurrent;

namespace Services
{
    public class NotificationServices
    {
        
        private readonly NotificationRepository _notificationRepository;

        public NotificationServices( NotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }
        public PaginationApiResponse<List<NotificationDTO>> GetNotification(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var notification = _notificationRepository.GetAllOrderedByExpression(no => no.UserId == userId, pageSize, pageNumber, query => query.OrderByDescending(n => n.CreatedAt))
                                                        .Select(no => new NotificationDTO
                                                        {
                                                            Title = no.Title,
                                                            Message = no.Message,
                                                            IsRead = no.IsRead,
                                                            CreatedAt = no.CreatedAt,
                                                        }).ToList();

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
