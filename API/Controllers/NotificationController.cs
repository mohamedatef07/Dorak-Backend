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
        private readonly UserManager<User> userManager;

        public NotificationController(NotificationServices notificationServices, UserManager<User> userManager)
        {
            _notificationServices = notificationServices;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("RegisterUserToNotificationHub")]
        public async Task<IActionResult> RegisterUserToNotificationHub(NotificationRequestDTO request)
        {

            var userInfo = await userManager.GetUserAsync(HttpContext.User);
            if (userInfo != null)
            {
                var userRole = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
                var userId = userInfo.Id;

                await _notificationServices.AddToUserNotificationHub(request.ConnectionId,
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
        public IActionResult GetNotifications([FromQuery] int pageNumber=1,[FromQuery] int pageSize=10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<object> { Message = "You are not authorized to perform this action", Status = 401 });
            }
            var notifications = notificationServices.GetNotification(userId,pageNumber,pageSize);
            if (notifications == null || !notifications.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "notifications not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<NotificationDTO>> { Message = "Get general statistics successfully", Status = 200, Data = notifications });
        }
    }
}
