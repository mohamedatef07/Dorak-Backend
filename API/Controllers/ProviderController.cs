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
        public ShiftServices shiftServices;
        public ProviderController(ProviderServices _providerServices,ShiftServices _shiftServices)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
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

       




    }
}
