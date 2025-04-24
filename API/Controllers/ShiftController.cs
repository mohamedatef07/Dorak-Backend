using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;
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


        [HttpGet("GetShifts")]
        public IActionResult GetShifts(DateOnly Date, int CenterId)
        {
            var shifts = shiftServices.GetShiftsWithDateAndCenterId(Date, CenterId);
            if (shifts != null)
            {
                return Ok(new ApiResponse<IQueryable<ShiftDTO>> { Status = 200, Message = "Successfully retrive Data", Data = shifts });

            }
            return Ok(new ApiResponse<ShiftDTO> { Status = 400, Message = "Error on retriving Data" });
        }

        [HttpGet("GetAppointment")]
        public IActionResult GetAppointment(int ShiftId)
        {
            var Appointments = shiftServices.GetAppointmentByShiftId(ShiftId);
            if (Appointments != null)
            {
                return Ok(new ApiResponse<IQueryable<AppointmentDTO>> { Status = 200, Message = "Successfully retrive Data", Data = Appointments });

            }
            return Ok(new ApiResponse<AppointmentDTO> { Status = 400, Message = "Error on retriving Data" });

        }
    }
}