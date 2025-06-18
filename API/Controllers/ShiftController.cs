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

        [HttpGet("get-shifts")]
        public IActionResult GetShifts([FromQuery] DateOnly date, [FromQuery] int centerId)
        {
            if (centerId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid center id provided", Status = 400 });
            }
            if (date == DateOnly.MinValue)
            {
                return BadRequest(new ApiResponse<object> { Message = "Date is required and must be a valid date.", Status = 400 });
            }
            var shifts = shiftServices.GetShiftsWithDateAndCenterId(date, centerId);
            if (shifts == null || !shifts.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "center not found", Status = 404 });
            }
            return Ok(new ApiResponse<IQueryable<ShiftDTO>> { Status = 200, Message = "Get shifts successfully", Data = shifts });
        }

        [HttpGet("get-shift-appointments")]
        public IActionResult GetShiftAppointments([FromQuery] int ShiftId)
        {
            if (ShiftId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid shift id provided", Status = 400 });

            }
            var appointments = shiftServices.GetAppointmentsByShiftId(ShiftId);
            if (appointments == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Shift not found" , Status = 404 });
            }
            if (!appointments.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "No appointments found", Status = 404 });
            }
            return Ok(new ApiResponse<IQueryable<AppointmentDTO>> { Message = "get shift appointments successfully", Status = 200, Data = appointments });
        }
        [HttpGet("get-all-center-shifts")]
        public IActionResult GetAllCenterShifts([FromQuery] int centerId)
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
            var allCenterShifts = shiftServices.GetAllCenterShifts(center);
            if (allCenterShifts == null || !allCenterShifts.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "No shifts found for this center", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetAllCenterShiftsDTO>> { Message = "Get all center shifts successfully", Status = 200, Data = allCenterShifts });
        }

        [HttpGet("get-all-centerShifts-and-services")]
        public IActionResult GetAllCenterShiftsAndServices([FromQuery] int centerId)
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