//using Dorak.DataTransferObject;
//using Dorak.Models;
//using Dorak.Models.Models.Wallet;
//using Dorak.ViewModels;
//using Dorak.ViewModels.DoctorCardVMs;
//using Microsoft.AspNetCore.Mvc;
//using Services;

//namespace API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ProviderController : ControllerBase
//    {
//        public ProviderServices providerServices;
//        public ProviderCardService providerCardService;
//        public ProviderController(ProviderServices _providerServices, ProviderCardService providerCardService)
//        {
//            providerServices = _providerServices;
//            this.providerCardService = providerCardService;
//        }
//        [HttpGet("MainInfo")]
//        public IActionResult ProviderMainInfo([FromQuery] string providerId)
//        {
//            if (string.IsNullOrWhiteSpace(providerId))
//            {
//                return BadRequest(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider ID is required", Status = 400 });
//            }
//            var provider = providerServices.GetProviderById(providerId);

//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider not found", Status = 404 });
//            }
//            GetProviderMainInfoDTO providerVM = providerServices.GetProviderMainInfo(provider);
//            return Ok(new ApiResponse<GetProviderMainInfoDTO>
//            {
//                Message = "Get Provider Info Successfully",
//                Status = 200,
//                Data = providerVM
//            });
//        }
//        [HttpGet("BookingInfo")]
//        public IActionResult ProviderBookingInfo([FromQuery]string providerId)
//        {
//            if (string.IsNullOrWhiteSpace(providerId))
//            {
//                return BadRequest(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider ID is required", Status = 400 });
//            }
//            Provider provider = providerServices.GetProviderById(providerId);
//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider not found", Status = 404 });
//            }
//            List<GetProviderBookingInfoDTO> providerBookingInfo = providerServices.GetProviderBookingInfo(provider);
//            if (providerBookingInfo == null || !providerBookingInfo.Any())
//            {
//                return NotFound(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider booking info not found", Status = 404 });
//            }
//            return Ok(new ApiResponse<List<GetProviderBookingInfoDTO>>
//            {
//                Message = "Get Provider  Booking Info Successfully",
//                Status = 200,
//                Data = providerBookingInfo
//            });
//        }
//        [HttpGet("ScheduleDetails")]
//        public IActionResult ScheduleDetails([FromQuery] string providerId)
//        {
//            if (string.IsNullOrWhiteSpace(providerId))
//            {
//                return BadRequest(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider ID is required", Status = 400 });
//            }
//            Provider provider = providerServices.GetProviderById(providerId);
//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider not found", Status = 404 });
//            }
//            List<GetProviderScheduleDetailsDTO> scheduleDetails = providerServices.GetScheduleDetails(provider);
//            if (scheduleDetails == null || !scheduleDetails.Any())
//            {
//                return NotFound(new ApiResponse<List<GetProviderScheduleDetailsDTO>> { Message = "Provider schedule details not found", Status = 404 });
//            }
//            return Ok(new ApiResponse<List<GetProviderScheduleDetailsDTO>>
//            {
//                Message = "Get Provider schedule details Successfully",
//                Status = 200,
//                Data = scheduleDetails
//            });
//        }
//        [HttpGet("AllScheduleDetails")]
//        public IActionResult AllScheduleDetails([FromQuery] string providerId)
//        {
//            if (string.IsNullOrWhiteSpace(providerId))
//            {
//                return BadRequest(new ApiResponse<List<GetAllProviderScheduleDetailsDTO>> { Message = "Provider ID is required", Status = 400 });
//            }
//            Provider provider = providerServices.GetProviderById(providerId);
//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<List<GetAllProviderScheduleDetailsDTO>> { Message = "Provider not found", Status = 404 });
//            }
//            List<GetAllProviderScheduleDetailsDTO> allScheduleDetails = providerServices.GetAllScheduleDetails(provider);
//            if (allScheduleDetails == null || !allScheduleDetails.Any())
//            {
//                return NotFound(new ApiResponse<List<GetAllProviderScheduleDetailsDTO>> { Message = "Provider schedule details not found", Status = 404 });
//            }
//            return Ok(new ApiResponse<List<GetAllProviderScheduleDetailsDTO>>
//            {
//                Message = "Get Provider schedule details Successfully",
//                Status = 200,
//                Data = allScheduleDetails
//            });
//        }

//        [HttpGet("cards")]
//        public IActionResult GetDoctorCards()
//        {
//            var doctors = providerCardService.GetDoctorCards();
//            return Ok(new ApiResponse<List<ProviderCardViewModel>>
//            {
//                Message = "Cards are displayed.",
//                Status = 200,
//                Data = doctors
//            });
//        }

//        [HttpGet("search")]
//        public IActionResult SearchDoctors(
//           [FromQuery] string? searchText,
//           [FromQuery] string? city,
//           [FromQuery] string? specialization)
//        {
//            var doctors = providerCardService.SearchDoctors(searchText, city, specialization);
//            return Ok(new ApiResponse<List<ProviderCardViewModel>>
//            {
//                Message = "Search Done Successfully",
//                Status = 200,
//                Data = doctors
//            });
//        }

//        [HttpGet("filter-by-day")]
//        public IActionResult FilterByDay([FromQuery] string day)
//        {
//            if (string.IsNullOrWhiteSpace(day))
//            {
//                return BadRequest(new ApiResponse<List<ProviderCardViewModel>>
//                {
//                    Message = "Day is required",
//                    Status = 400,
//                    Data = new List<ProviderCardViewModel>()  
//                });
//            }

//            var doctors = providerCardService.FilterByDay(day);

//            return Ok(new ApiResponse<List<ProviderCardViewModel>>
//            {
//                Message = $"Doctors available on {day} retrieved successfully.",
//                Status = 200,
//                Data = doctors
//            });
//        }
//    }
//}
