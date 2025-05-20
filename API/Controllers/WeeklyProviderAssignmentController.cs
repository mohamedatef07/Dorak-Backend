using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WeeklyProviderAssignmentController : Controller
    {

            private readonly ProviderServices providerService;

            public WeeklyProviderAssignmentController(ProviderServices _providerService)
            {
                providerService = _providerService;
            }

            //[HttpPost("assignweekly")]
            //public IActionResult AssignWeekly([FromBody] WeeklyProviderAssignmentViewModel model)
            //{
            //   // var result = providerService.AssignProviderToCenterWithWorkingDays(model);
            //   // return Ok(new { message = result });
            //}
        }
    }


