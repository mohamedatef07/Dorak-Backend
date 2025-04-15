using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;
using Dorak.ViewModels.ShiftViewModel;

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

        [HttpPost("create")]
        public IActionResult ManageProviderSchedule([FromBody] ShiftViewModel model)
        {

            var result = providerServices.CreateShift(model);

            return Ok(new { message = result });
        }

        [HttpGet("GetShifts")]
        public IActionResult GetShifts(DateOnly Date, int CenterId)
        {
            var shifts = shiftServices.GetShiftsWithDateAndCenterId(Date, CenterId);
            if (shifts != null)
            {
                return Ok( new ApiResponse<IQueryable<ShiftDTO>> { Status = 200, Message = "Successfully retrive Data", Data = shifts });
                
            }
            return Ok( new ApiResponse<ShiftDTO> { Status = 400, Message = "Error on retriving Data"});
        }
    }
}
