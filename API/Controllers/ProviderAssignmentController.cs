using Microsoft.AspNetCore.Mvc;
using Services;
using Dorak.ViewModels;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderAssignmentController : ControllerBase
    {
        private readonly ProviderServices providerServices;

        public ProviderAssignmentController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }



        [HttpPost]
        [Route("assign")]
        public IActionResult AssignProvider([FromBody] ProviderAssignmentViewModel model)
        {

            var result = providerServices.AssignProviderToCenter(model);

            return Ok(new { message = result });
        }
    }
}
