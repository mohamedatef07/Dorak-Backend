using Dorak.DataTransferObject;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.Data.Entity.Core.Common;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly OperatorServices operatorServices;
        private readonly AppointmentServices appointmentServices;
        private readonly ShiftServices shiftServices;
        private readonly ProviderServices providerServices;
        private readonly LiveQueueServices liveQueueServices;

        public OperatorController(OperatorServices _operatorServices, AppointmentServices _appointmentServices, ShiftServices _shiftServices, ProviderServices _providerServices, LiveQueueServices _liveQueueServices)
        {
            operatorServices = _operatorServices;
            appointmentServices = _appointmentServices;
            shiftServices = _shiftServices;
            providerServices = _providerServices;
            liveQueueServices = _liveQueueServices;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = operatorServices.GetAllOperators();
            if (result != null)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfull get of operators" });
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
        public IActionResult RestoreOperator(string operatorId)
        {
            var result = operatorServices.RestoreOperator(operatorId);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Restored." });
            }
            return BadRequest(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Unsuccessfully Restored." });
        }

        [HttpGet("start-shift")]
        public IActionResult StartShift([FromQuery] int shiftId, [FromQuery] string operatorId)
        {
            if (shiftId <= 0 || operatorId.IsNullOrEmpty())
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Shift ID or Operator ID format provided", Status = 400 });
            }
            var result = operatorServices.StartShift(shiftId, operatorId);
            if (!result)
            {
                return NotFound(new ApiResponse<object> { Message = "Failed to start shift", Status = 400 });
            }
            return Ok(new ApiResponse<object> { Message = "Shift Started Successfully", Status = 200 });
        }

        [HttpGet("end-shift")]
        public IActionResult EndShift([FromQuery] int shiftId, [FromQuery] string operatorId)
        {
            var result = operatorServices.EndShift(shiftId, operatorId);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully End Shift." });
            }
            return BadRequest(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Failed to End Shift." });
        }
        [HttpGet("cancel-shift")]
        public IActionResult CancelShift([FromQuery] int shiftId, [FromQuery] int centerId)
        {
            if (shiftId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Shift ID Provided", Status = 400 });
            }
            var shift = shiftServices.GetShiftById(shiftId);
            if (shift == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Shift is not found", Status = 404 });
            }
            var isCanceled = shiftServices.ShiftCancelation(shift, centerId);
            if (!isCanceled.Result)
            {
                return BadRequest(new ApiResponse<object> { Message = "Failed to cancel shift", Status = 400 });
            }
            return Ok(new ApiResponse<object> { Message = "Shift canceled successfully", Status = 200 });
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

        // [Authorize(Roles = "Admin, Operator")]
        [HttpGet]
        [Route("GetProviderLiveQueues")]
        public IActionResult GetProviderLiveQueues(string providerId, int centerId, int shiftId, int pageNumber = 1, int pageSize = 16)
        {
            try
            {
                // Validate providerId
                if (string.IsNullOrEmpty(providerId))
                {
                    return BadRequest(new ApiResponse<ProviderLiveQueueViewModel>
                    {
                        Message = "Provider Id is required",
                        Status = 400
                    });
                }

                // Check if provider exists
                var provider = providerServices.GetProviderById(providerId);
                if (provider == null)
                {
                    return NotFound(new ApiResponse<ProviderLiveQueueViewModel>
                    {
                        Message = "Provider not found",
                        Status = 404
                    });
                }

                // Fetch live queues for the specific provider, center, and shift
                var liveQueues = liveQueueServices.GetLiveQueuesForProvider(providerId, centerId, shiftId, pageNumber, pageSize);

                if (liveQueues.Data == null || !liveQueues.Data.Any())
                {
                    return NotFound(new ApiResponse<PaginationViewModel<ProviderLiveQueueViewModel>>
                    {
                        Message = "No live queues found for the specified provider, center, and shift",
                        Status = 404,
                        Data = new PaginationViewModel<ProviderLiveQueueViewModel>
                        {
                            Data = new List<ProviderLiveQueueViewModel>(),
                            PageNumber = pageNumber,
                            PageSize = pageSize,
                            Total = 0
                        }
                    });
                }

                return Ok(new ApiResponse<PaginationViewModel<ProviderLiveQueueViewModel>>
                {
                    Data = liveQueues,
                    Message = "Live queues retrieved successfully",
                    Status = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}",
                    Status = 500
                });
            }
        }

        // [Authorize(Roles = "Admin, Operator")]
        [HttpPost]
        [Route("UpdateLiveQueueStatus")]
        public async Task<IActionResult> UpdateLiveQueueStatus([FromBody] UpdateQueueStatusViewModel model, decimal additionalFees)
        {
            try
            {
                var result = await operatorServices.UpdateQueueStatusAsync(model, additionalFees);
                if (result.StartsWith("Queue status updated successfully"))
                {
                    return Ok(new ApiResponse<string>
                    {
                        Data = result,
                        Message = "Success",
                        Status = 200
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse<string>
                    {
                        Data = result,
                        Message = "Failed",
                        Status = 400
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Data = null,
                    Message = $"Internal Server Error: {ex.Message}",
                    Status = 500
                });
            }
        }

    }
}
