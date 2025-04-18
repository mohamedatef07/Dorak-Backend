using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;
using Dorak.ViewModels.ShiftViewModel;
using Dorak.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly ProviderServices providerServices;
        private readonly ShiftServices shiftServices;

        public ShiftController(ProviderServices _providerServices, ShiftServices _shiftServices)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
        }

        //[HttpPost("create")]
        //public IActionResult ManageProviderSchedule([FromBody] ShiftViewModel model)
        //{

        //    var result = providerServices.CreateShift(model);

        //    return Ok(new { message = result });
        //}
    }
}
