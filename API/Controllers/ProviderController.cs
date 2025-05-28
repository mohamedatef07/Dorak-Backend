using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;

namespace API.Controllers
{
    // [Authorize(Roles = "Provider")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        public ProviderServices providerServices;
        public ShiftServices shiftServices;
        public LiveQueueServices liveQueueServices;
        public ProviderController(ProviderServices _providerServices, ShiftServices _shiftServices, LiveQueueServices _liveQueueServices)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            liveQueueServices = _liveQueueServices;
        }

        [HttpGet("GetProviderById/{providerId}")]
        public async Task<IActionResult> GetProviderById(string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<ProviderViewModel> { Message = "Provider ID is required", Status = 400 });
            }

            try
            {
                var provider = await providerServices.GetProviderDetailsById(providerId);
                if (provider == null)
                {
                    return NotFound(new ApiResponse<ProviderViewModel> { Message = "Provider not found", Status = 404 });
                }

                return Ok(new ApiResponse<ProviderViewModel>
                {
                    Message = "Provider details retrieved successfully",
                    Status = 200,
                    Data = provider
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<ProviderViewModel> { Message = "Internal server error", Status = 500 });
            }
        }

        [HttpGet("{providerId}/assignments")]
        public IActionResult GetProviderAssignments(string providerId, int centerId)
        {
            try
            {
                var assignments = providerServices.GetProviderAssignments(providerId, centerId);

                if (!assignments.Any() || assignments == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Message = "No assignments found for the given criteria.",
                        Status = 404
                    });
                }

                return Ok(new ApiResponse<object>
                {
                    Message = "Assignments retrieved successfully",
                    Status = 200,
                    Data = assignments.ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Message = "Internal server error",
                    Status = 500
                });
            }
        }
        [HttpGet("schedule-details")]
        public IActionResult ScheduleDetails([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<object> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderScheduleDetailsDTO> scheduleDetails = providerServices.GetScheduleDetails(provider);
            if (scheduleDetails == null || !scheduleDetails.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "Provider schedule details not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderScheduleDetailsDTO>>
            {
                Message = "Get Provider schedule details Successfully",
                Status = 200,
                Data = scheduleDetails
            });
        }
        [HttpGet("shift-details")]
        public IActionResult ShiftDetails([FromQuery] int shiftId)
        {
            if (shiftId <= 0)
            {
                return BadRequest(new ApiResponse<GetShiftDetailsDTO>
                {
                    Message = "Invalid shift id",
                    Status = 400
                });
            }
            GetShiftDetailsDTO shiftDetails = providerServices.GetShiftDetails(shiftId);
            if (shiftDetails == null)
            {
                return NotFound(new ApiResponse<GetShiftDetailsDTO> { Message = "Shift details not found", Status = 404 });
            }
            return Ok(new ApiResponse<GetShiftDetailsDTO>
            {
                Message = "Get shift details Successfully",
                Status = 200,
                Data = shiftDetails
            });
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateDoctorProfile([FromBody] UpdateProviderProfileDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid data", Status = 400 });
            }
            var result = await providerServices.UpdateDoctorProfile(model);

            return Ok(new ApiResponse<object>
            {
                Message = result,
                Status = 200,
            });
        }
        [HttpPut("update-professional-info")]
        public IActionResult UpdateProfessionalInfo([FromBody] UpdateProviderProfessionalInfoDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid data", Status = 400 });
            }
            var result = providerServices.UpdateProfessionalInfo(model);
            if(result == false)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Provider not found",
                    Status= 404,
                });
            }
            return Ok(new ApiResponse<object>
            {
                Message = "Updated professional info successfuly",
                Status = 200,
            });
        }
        [HttpGet("Queue-Entries")]
        public IActionResult QueueEntries(string providerId)
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new ApiResponse<object> { Message = "Provider Id is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
            }
            List<GetQueueEntriesDTO> queueEntries = liveQueueServices.GetQueueEntries(provider);
            if (queueEntries == null || !queueEntries.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "Queue entries not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetQueueEntriesDTO>>
            {
                Message = "Get queue entries successfully",
                Status = 200,
                Data = queueEntries
            });
        }
    }
}
