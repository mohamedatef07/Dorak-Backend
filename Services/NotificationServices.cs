using Data;
using Dorak.DataTransferObject;
using Repositories;

namespace Services
{
    public class NotificationServices
    {

        private readonly NotificationRepository _notificationRepository;
        private readonly CommitData commitData;

        public NotificationServices(NotificationRepository notificationRepository,
            CommitData _commitData)
        {
            _notificationRepository = notificationRepository;
            commitData = _commitData;
        }
        public PaginationApiResponse<List<NotificationDTO>> GetNotification(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var notification = _notificationRepository.GetAllOrderedByExpression(no => no.UserId == userId, pageSize, pageNumber, query => query.OrderByDescending(n => n.CreatedAt))
                                                        .Select(no => new NotificationDTO
                                                        {
                                                            NotificationId = no.NotificationId,
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
        public void MarkNotificationAsRead(int notificationId)
        {
            var notification = _notificationRepository.GetById(n => n.NotificationId == notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _notificationRepository.Edit(notification);
                commitData.SaveChanges();

            }
        }
    }
}
