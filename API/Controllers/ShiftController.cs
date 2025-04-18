using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly ProviderServices providerServices;

        public ShiftController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }

        //[HttpPost("create")]
        //public IActionResult ManageProviderSchedule([FromBody] ShiftViewModel model)
        //{

        //    var result = providerServices.CreateShift(model);

        //    return Ok(new { message = result });
        //}
    }
}
