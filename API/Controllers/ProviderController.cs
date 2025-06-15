using System.Security.Claims;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Data.Entity.Core.Common;

namespace API.Controllers
{
    // [Authorize(Roles = "Provider")]
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        private readonly CenterServices centerServices;
        private readonly ProviderServices providerServices;
        private readonly OperatorServices operatorServices;
        private readonly ShiftServices shiftServices;
        private readonly LiveQueueServices liveQueueServices;
        private readonly LiveQueueRepository liveQueueRepository;
        public ProviderController(ProviderServices _providerServices, ShiftServices _shiftServices, LiveQueueServices _liveQueueServices, CenterServices _centerServices, OperatorServices _operatorServices, LiveQueueRepository _liveQueueRepository)
        {
            centerServices = _centerServices;
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            liveQueueServices = _liveQueueServices;
            operatorServices = _operatorServices;
            liveQueueRepository = _liveQueueRepository;

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

        [Authorize(Roles = "Provider")]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateProviderProfileDTO model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("User not authorized.");

            var result = await providerServices.UpdateDoctorProfile(userId, model);
            return Ok(new { message = result });
        }


        [Authorize(Roles = "Provider")]
        [HttpPut("update-professional-info")]
        public IActionResult UpdateProfessionalInfo([FromForm] UpdateProviderProfessionalInfoDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid data", Status = 400 });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<object> { Message = "Unauthorized", Status = 401 });
            }

            var result = providerServices.UpdateProfessionalInfo(userId, model);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Provider not found",
                    Status = 404,
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Updated professional info successfully",
                Status = 200,
            });
        }
        //[HttpGet("queue-entries")]
        //public IActionResult GetQueueEntries([FromQuery]string providerId)
        //{
        //    if (string.IsNullOrEmpty(providerId))
        //    {
        //        return BadRequest(new ApiResponse<object> { Message = "Provider Id is required", Status = 400 });
        //    }
        //    Provider provider = providerServices.GetProviderById(providerId);
        //    if (provider == null)
        //    {
        //        return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
        //    }
        //    List<GetQueueEntriesDTO> queueEntries = liveQueueServices.GetQueueEntries(provider);
        //    if (queueEntries == null || !queueEntries.Any())
        //    {
        //        return NotFound(new ApiResponse<object> { Message = "Queue entries not found", Status = 404 });
        //    }
        //    return Ok(new ApiResponse<List<GetQueueEntriesDTO>>
        //    {
        //        Message = "Get queue entries successfully",
        //        Status = 200,
        //        Data = queueEntries
        //    });
        //}

        [HttpGet("ProviderProfile")]
        [Authorize(Roles = "Provider")]
        public IActionResult GetMyProviderProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ApiResponse<string>
                {
                    Message = "User ID is required",
                    Status = 400,
                    Data = null
                });
            }

            var profile = providerServices.GetProviderProfile(userId);

            if (profile == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Provider not found",
                    Status = 404,
                    Data = null
                });
            }

            return Ok(new ApiResponse<ProviderProfileDTO>
            {
                Message = "Provider profile retrieved successfully",
                Status = 200,
                Data = profile
            });
        }
    }
}
