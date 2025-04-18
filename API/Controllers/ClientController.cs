using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ShiftDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        public ProviderServices providerServices;
        public ProviderCardService providerCardService;
        public ShiftServices shiftServices;
        public ClientController(ProviderServices _providerServices, ProviderCardService providerCardService, ShiftServices _shiftServices)
        {
            providerServices = _providerServices;
            this.providerCardService = providerCardService;
            shiftServices = _shiftServices;
        }
        [HttpGet("MainInfo")]
        public IActionResult ProviderMainInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider ID is required", Status = 400 });
            }
            var provider = providerServices.GetProviderById(providerId);

            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider not found", Status = 404 });
            }
            GetProviderMainInfoDTO providerVM = providerServices.GetProviderMainInfo(provider);
            return Ok(new ApiResponse<GetProviderMainInfoDTO>
            {
                Message = "Get Provider Info Successfully",
                Status = 200,
                Data = providerVM
            });
        }
        [HttpGet("BookingInfo")]
        public IActionResult ProviderBookingInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderBookingInfoDTO> providerBookingInfo = providerServices.GetProviderBookingInfo(provider);
            if (providerBookingInfo == null || !providerBookingInfo.Any())
            {
                return NotFound(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider booking info not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderBookingInfoDTO>>
            {
                Message = "Get Provider  Booking Info Successfully",
                Status = 200,
                Data = providerBookingInfo
            });
        }

        [HttpGet("CenterServices")]
        public IActionResult GetCenterServices([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetCenterServicesShiftDTO> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<GetCenterServicesShiftDTO> { Message = "Provider not found", Status = 404 });
            }
            List<GetCenterServicesShiftDTO> centerServices = providerServices.GetCenterServices(provider);
            if (centerServices == null || !centerServices.Any())
            {
                return NotFound(new ApiResponse<GetCenterServicesShiftDTO> { Message = "Provider center services not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetCenterServicesShiftDTO>>
            {
                Message = "Get Provider  center services Successfully",
                Status = 200,
                Data = centerServices
            });
        }
    }
}
