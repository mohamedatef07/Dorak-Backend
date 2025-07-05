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
        private readonly UserManager<User> userManager;

        public NotificationController(NotificationServices notificationServices, UserManager<User> userManager, NotificationSignalRService notificationSignalRService)
        {
            _notificationServices = notificationServices;
            this.userManager = userManager;
            _notificationSignalRService = notificationSignalRService;
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
                return NotFound(new PaginationApiResponse<object>(false, "notifications not found", 404, null, 0, pageNumber, pageSize));
            }
            return Ok(paginationResponse);
        }
    }
}
