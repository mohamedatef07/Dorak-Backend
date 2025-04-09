//using Dorak.Models;
//using Dorak.ViewModels;
//using Microsoft.AspNetCore.Mvc;
//using Services;

//namespace API.Controllers
//{
//    [ApiController]
//    [Route("api/{controller}")]
//    public class ProviderController : ControllerBase
//    {
//        public ProviderServices providerServices;
//        public ProviderController(ProviderServices _providerServices)
//        {
//            providerServices = _providerServices;
//        }
//        [HttpGet("ProviderInfo")]
//        public IActionResult ProviderMainInfo(string id)
//        {
//            if (string.IsNullOrWhiteSpace(id))
//            {
//                return BadRequest(new ApiResponse<GetProviderMainInfoViewModel> { Message = "Provider ID is required", Status = 400 });
//            }

//            var provider = providerServices.GetProviderById(id);

//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<GetProviderMainInfoViewModel> { Message = "Provider not found", Status = 404 });
//            }
//            var providerVM = new GetProviderMainInfoViewModel
//            {
//                FirstName = provider.FirstName,
//                LastName = provider.LastName,
//                Specialization = provider.Specialization,
//                Bio = provider.Bio,
//                Rate = provider.Rate,
//                Image = provider.Image
//            };
//            return Ok(new ApiResponse<GetProviderMainInfoViewModel>
//            {
//                Message = "Get Provider Info Successfully",
//                Status = 200,
//                Data = providerVM
//            });
//        }
//        [HttpGet("BookingInfo")]
//        public IActionResult ProviderBookingInfo(string providerId)
//        {
//            if (string.IsNullOrWhiteSpace(providerId))
//            {
//                return BadRequest(new ApiResponse<GetProviderBookingInfoViewModel> { Message = "Provider ID is required", Status = 400 });
//            }
//            var provider = providerServices.GetProviderById(providerId);
//            if (provider == null)
//            {
//                return NotFound(new ApiResponse<GetProviderBookingInfoViewModel> { Message = "Provider not found", Status = 404 });
//            }
//            //var providerBookingInfo = providerServices.GetProviderBookingInfo(providerId);
//            //return Ok(new ApiResponse<GetProviderBookingInfoViewModel>
//            //{
//            //    Message = "Get Provider  Booking Info Successfully",
//            //    Status = 200,
//            //    Data = providerBookingInfo
//            //});
//        }

//    }
//}
