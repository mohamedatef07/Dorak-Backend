using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProviderController : ControllerBase
    {
        public ProviderServices providerServices;
        public ProviderCardService providerCardService;
        public ShiftServices shiftServices;
        public LiveQueueServices liveQueueServices;
        public ProviderController(ProviderServices _providerServices, ProviderCardService providerCardService, ShiftServices _shiftServices, LiveQueueServices _liveQueueServices)
        {
            providerServices = _providerServices;
            providerCardService = providerCardService; 
            shiftServices = _shiftServices;
            liveQueueServices = _liveQueueServices;
        }
        [HttpGet("ScheduleDetails")]
        public IActionResult ScheduleDetails([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderScheduleDetailsDTO> scheduleDetails = providerServices.GetScheduleDetails(provider);
            if (scheduleDetails == null || !scheduleDetails.Any())
            {
                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider schedule details not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderScheduleDetailsDTO>>
            {
                Message = "Get Provider schedule details Successfully",
                Status = 200,
                Data = scheduleDetails
            });
        }
        [HttpGet("ShiftDetails")]
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

        [HttpGet("cards")]
        public IActionResult GetDoctorCards()
        {
            var doctors = providerCardService.GetDoctorCards();
            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Cards are displayed.",
                Status = 200,
                Data = doctors
            });
        }

        [HttpGet("search")]
        public IActionResult SearchDoctors(
           [FromQuery] string? searchText,
           [FromQuery] string? city,
           [FromQuery] string? specialization)
        {
            var doctors = providerCardService.SearchDoctors(searchText, city, specialization);
            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Search Done Successfully",
                Status = 200,
                Data = doctors
            });
        }

        [HttpGet("filter-by-day")]
        public IActionResult FilterByDay([FromQuery] DateOnly date)
        {
            var doctors = providerCardService.FilterByDay(date);

            if (!doctors.Any())
            {
                return NotFound(new ApiResponse<List<ProviderCardViewModel>>
                {
                    Message = "Day is required",
                    Status = 400,
                    Data = new List<ProviderCardViewModel>()
                });
            }

            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = $"Doctors available on {date} retrieved successfully.",
                Status = 200,
                Data = doctors
            });
        }

        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateDoctorProfile([FromBody] UpdateProviderProfileDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Message = "Invalid data", Status = 400 });

            var result = await providerServices.UpdateDoctorProfile(model);

            return Ok(new ApiResponse<string>
            {
                Message = result,
                Status = 200,
                Data = result
            });
        }

        [HttpPut("update-professional-info")]
        public IActionResult UpdateProfessionalInfo([FromBody] UpdateProviderProfessionalInfoDTO model)
        {
            var result = providerServices.UpdateProfessionalInfo(model);

            return Ok(new ApiResponse<string>
            {
                Message = result,
                Status = 200,
                Data = result
            });
        }
        [HttpGet("queue-entries")]
        public IActionResult QueueEntries(string providerId)
        {
            if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest(new ApiResponse<GetQueueEntriesDTO> { Message = "Provider Id is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<List<GetQueueEntriesDTO>> { Message = "Provider not found", Status = 404 });
            }
            List<GetQueueEntriesDTO> queueEntries = liveQueueServices.GetQueueEntries(provider);
            if (queueEntries == null || !queueEntries.Any())
            {
                return NotFound(new ApiResponse<List<GetQueueEntriesDTO>> { Message = "Queue entries not found", Status = 404 });
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
