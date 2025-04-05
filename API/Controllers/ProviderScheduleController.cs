using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderScheduleController : ControllerBase
    {
        private readonly ProviderServices providerServices;

        public ProviderScheduleController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }

        [HttpPost("schedule")]
        public IActionResult ManageProviderSchedule([FromBody] ProviderScheduleViewModel model)
        {

            var result = providerServices.ManageProviderSchedule(model);

            return Ok(new { message = result });
        }
    }
}
