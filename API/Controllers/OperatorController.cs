using Dorak.DataTransferObject;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly OperatorServices operatorServices;
        private readonly AppointmentServices appointmentServices;

        public OperatorController(OperatorServices _operatorServices,AppointmentServices _appointmentServices) 
        {
            operatorServices = _operatorServices;
            appointmentServices = _appointmentServices;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = operatorServices.GetAllOperators();
            if (result != null) 
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfull get of operators"});
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "No operators exist" });
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string operatorid)
        {
            var result = await operatorServices.DeleteOperator(operatorid);
            if (result)
            {
                return Ok(new ApiResponse<object> { Message = "Deleted successfully", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Delete failed", Status = 400 });
        }

        [HttpGet("Restore")]
        public IActionResult RestoreOperator(string operatorid) 
        {
            var result = operatorServices.RestoreOperator(operatorid);
            if (result == true) 
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Restored." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Unsuccessfully Restored." });
        }

        [HttpGet("Startshift")]
        public IActionResult StartShift(int shiftId, string operatorid) 
        {
            var result = operatorServices.StartShift(shiftId, operatorid);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Started." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Failed to Start." });
        }

        [HttpGet("EndShift")]
        public IActionResult EndShift(int shiftId, string operatorid)
        {
            var result = operatorServices.EndShift(shiftId, operatorid);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully End Shift." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Failed to End Shift." });
        }

        [HttpPost("reserve-appointment")]
        public IActionResult ReserveAppointment([FromForm] ReserveApointmentDTO reserveApointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = "Invalid appointment data"
                });
            }
            try
            {
                var appointment = operatorServices.CreateAppointment(reserveApointmentDTO);

                return Ok(new ApiResponse<object>
                {
                    Status = 200,
                    Message = "Appointment reserved successfully",
                    Data = appointment
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 400,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = 500,
                    Message = "An error occurred while reserving the appointment",
                    Data = ex.Message 
                });
            }
        }

    }
}
