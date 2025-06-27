using Dorak.DataTransferObject;
using Dorak.Models;
using Microsoft.AspNetCore.Http;
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
        private readonly NotificationServices notificationServices;
  
        private readonly UserManager<User> userManager;

        public NotificationController(NotificationServices notificationServices,  UserManager<User> userManager)
        {
            this.notificationServices = notificationServices;
            this.userManager = userManager;
        }
        [HttpPost]
        [Route("RegisterUserToNotificationHub")]
        public async Task<IActionResult> RegisterUserToNotificationHub([FromBody]string ConnectionId)
        {

            var userInfo = await userManager.GetUserAsync(HttpContext.User);
            if (userInfo != null)
            {
                var userRole = HttpContext.User?.FindFirst(ClaimTypes.Role)?.Value;
                var userId = userInfo.Id;

                await notificationServices.AddToUserNotificationHub(ConnectionId,
                              new AddUserToNotificationHubDTO()
                              {
                                  UserId = userId,
                                  ConnectionId = ConnectionId,
                                  Role = userRole,
                                  Name = userInfo.UserName,


                              });
            }
            return Ok();

        }
    }
}
