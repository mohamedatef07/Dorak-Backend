using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.DoctorCardVMs;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ProviderController : ControllerBase
    {
        public ProviderServices providerServices;
        public ProviderCardService providerCardService;
        public ProviderController(ProviderServices _providerServices, ProviderCardService providerCardService)
        {
            providerServices = _providerServices;
            this.providerCardService = providerCardService;
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
        public IActionResult FilterByDay([FromQuery] string day)
        {
            if (string.IsNullOrWhiteSpace(day))
            {
                return BadRequest(new ApiResponse<List<ProviderCardViewModel>>
                {
                    Message = "Day is required",
                    Status = 400,
                    Data = new List<ProviderCardViewModel>()  
                });
            }

            var doctors = providerCardService.FilterByDay(day);

            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = $"Doctors available on {day} retrieved successfully.",
                Status = 200,
                Data = doctors
            });
        }






    }
}
