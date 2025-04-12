using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ProviderController : ControllerBase
    {
        public ProviderServices providerServices;
        public ProviderController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }
        [HttpGet("ProviderInfo")]
        public IActionResult ProviderMainInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new ApiResponse<GetProviderMainInfoViewModel> { Message = "Provider ID is required", Status = 400 });
            }
            var provider = providerServices.GetProviderById(id);

            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderMainInfoViewModel> { Message = "Provider not found", Status = 404 });
            }
            GetProviderMainInfoViewModel providerVM = providerServices.GetProviderMainInfo(provider);
            return Ok(new ApiResponse<GetProviderMainInfoViewModel>
            {
                Message = "Get Provider Info Successfully",
                Status = 200,
                Data = providerVM
            });
        }
        [HttpGet("BookingInfo")]
        public IActionResult ProviderBookingInfo(string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderBookingInfoViewModel> { Message = "Provider ID is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderBookingInfoViewModel> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderBookingInfoViewModel> providerBookingInfo = providerServices.GetProviderBookingInfo(provider);
            return Ok(new ApiResponse<List<GetProviderBookingInfoViewModel>>
            {
                Message = "Get Provider  Booking Info Successfully",
                Status = 200,
                Data = providerBookingInfo
            });
        }
    }
}
