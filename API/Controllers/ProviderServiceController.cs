using Microsoft.AspNetCore.Mvc;
using Dorak.ViewModels;
using Services;
using Data;
using Repositories;
using Dorak.DataTransferObject;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderCenterServiceController : ControllerBase
    {
        

        private readonly ProviderServices providerServices;

        public ProviderCenterServiceController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
         
        }

        [HttpPost("assign")]
        public IActionResult AssignServiceToCenter([FromBody] AssignProviderCenterServiceViewModel model)
        {

            var result = providerServices.AssignServiceToCenter(model);
            return Ok(new { message = result });
        }

       






    }
}
