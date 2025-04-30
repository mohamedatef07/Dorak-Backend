using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RescheduleAssignmentController : ControllerBase
    {
        private readonly ProviderServices providerServices;

        public RescheduleAssignmentController(ProviderServices providerServices)
        {
            this.providerServices = providerServices;
        }

        [HttpPost("reschedule")]
        public IActionResult RescheduleAssignment([FromBody] RescheduleAssignmentViewModel model)
        {
            var result = providerServices.RescheduleAssignment(model);
            return Ok(new { message = result });
        }
    }

}
