using Dorak.DataTransferObject;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly ProviderServices providerServices;
        private readonly ShiftServices shiftServices;
        private readonly CenterServices centerServices;

        public ShiftController(ProviderServices _providerServices, ShiftServices _shiftServices, CenterServices _centerServices)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            centerServices = _centerServices;
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
        [HttpGet("get-all-center-shifts")]
        public IActionResult GetAllCenterShifts([FromQuery]int centerId)
        {
            if(centerId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid center ID provided" ,Status=400});
            }
            var center = centerServices.GetCenterById(centerId);
            if (center == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Center not found", Status = 404 });
            }
            var allCenterShifts = shiftServices.GetAllCenterShifts(center);
            if(allCenterShifts == null || !allCenterShifts.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "No shifts found for this center", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetAllCenterShiftsDTO>> { Message = "Get all center shifts successfully", Status = 200 , Data = allCenterShifts});
        }

        [HttpGet("GetAllCenterShiftsAndServices")]
        public IActionResult GetAllCenterShiftsAndServices([FromQuery]int centerId)
        {
            if (centerId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid center ID provided", Status = 400 });
            }
            var center = centerServices.GetCenterById(centerId);
            if (center == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Center not found", Status = 404 });
            }
            var allCenterShifts = shiftServices.GetAllCenterShiftsAndServices(center);
            if (allCenterShifts == null || !allCenterShifts.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "No shifts found for this center", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetAllCenterShiftAndServicesDTO>> { Message = "Get all center shifts successfully", Status = 200, Data = allCenterShifts });
        }
    }
}