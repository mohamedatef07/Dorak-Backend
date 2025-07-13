using Dorak.DataTransferObject;
using Dorak.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationServices _notificationServices;
        private readonly NotificationSignalRService _notificationSignalRService;
        private readonly UserManager<User> _userManager;

        public NotificationController(NotificationServices notificationServices, UserManager<User> userManager, NotificationSignalRService notificationSignalRService)
        {
            _notificationServices = notificationServices;
            _userManager = userManager;
            _notificationSignalRService = notificationSignalRService;
        }

        [HttpPost]
        [Route("RegisterUserToNotificationHub")]
        public async Task<IActionResult> RegisterUserToNotificationHub(NotificationRequestDTO request)
        {
            var userInfo = await _userManager.GetUserAsync(HttpContext.User);
            if (userInfo != null)
            {
                var userRole = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
                var userId = userInfo.Id;
                await _notificationSignalRService.AddToUserNotificationHub(request.ConnectionId,
                              new AddUserToNotificationHubDTO()
                              {
                                  UserId = userId,
                                  ConnectionId = request.ConnectionId,
                                  Role = userRole,
                                  Name = userInfo.UserName,
                              });
            }
            return Ok();
        }

        [HttpGet("notifications")]
        public IActionResult GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new PaginationApiResponse<object>(false, "You are not authorized to perform this action", 401, null, 0, pageNumber, pageSize));
            }
            var paginationResponse = _notificationServices.GetNotification(userId, pageNumber, pageSize);
            if (paginationResponse.Data == null || !paginationResponse.Data.Any())
            {
                return Ok(new PaginationApiResponse<List<NotificationDTO>>(false, "notifications not found", 200, [], 0, pageNumber, pageSize));
            }
            return Ok(paginationResponse);
        }

        [HttpPost("MarkAsRead/{Id}")]
        public IActionResult MarkAsRead(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Notification id", Status = 400 });
            }
            var isModified = _notificationServices.MarkNotificationAsRead(Id);
            if (!isModified)
            {
                return NotFound(new ApiResponse<object> { Message = "Not found notification to mark it as read", Status = 404 });
            }
            return Ok(new ApiResponse<object> { Message = "Notifications is marked as is read successfully", Status = 200 });
        }
        [HttpPost("MarkAllAsRead")]
        public IActionResult MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<object> { Message = "You are not authorized to perform this action", Status = 401 });
            }
            var isModified = _notificationServices.MarkAllNotificationsAsRead(userId);
            if (!isModified)
            {
                return NotFound(new ApiResponse<object> { Message = "Not found notifications to mark it as read", Status = 404 });
            }
            return Ok(new ApiResponse<object> { Message = "All notifications are marked as is read successfully", Status = 200 });
        }
    }
}
