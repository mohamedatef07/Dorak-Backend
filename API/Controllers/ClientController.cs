using Data;
using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ClientDTO;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Repositories;
using Services;


namespace API.Controllers
{
    //[Authorize(Roles ="Client")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ProviderServices providerServices;
        private readonly ShiftServices shiftServices;
        private readonly AppointmentServices appointmentServices;
        private readonly Review_Service reviewService;
        public ClientController(AppointmentServices _appointmentServices, ProviderServices _providerServices, ShiftServices _shiftServices, Review_Service _reviewService)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            appointmentServices = _appointmentServices;
            reviewService = _reviewService;
        }
        [HttpGet("MainInfo")]
        public IActionResult ProviderMainInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider id is required", Status = 400 });
            }
            var provider = providerServices.GetProviderById(providerId);

            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderMainInfoDTO> { Message = "Provider not found", Status = 404 });
            }
            GetProviderMainInfoDTO mainInfo = providerServices.GetProviderMainInfo(provider);
            return Ok(new ApiResponse<GetProviderMainInfoDTO>
            {
                Message = "Get Provider Info Successfully",
                Status = 200,
                Data = mainInfo
            });
        }
        [HttpGet("BookingInfo")]
        public IActionResult ProviderBookingInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderBookingInfoDTO> { Message = "Provider id is required", Status = 400 });
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

        [HttpGet("ProviderCenterServices")]
        public IActionResult ProviderCenterServices([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<GetProviderCenterServicesDTO> { Message = "Provider id is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<GetProviderCenterServicesDTO> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderCenterServicesDTO> centerServices = providerServices.GetCenterServices(provider);
            if (centerServices == null || !centerServices.Any())
            {
                return NotFound(new ApiResponse<GetProviderCenterServicesDTO> { Message = "Provider center services not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderCenterServicesDTO>>
            {
                Message = "Get provider center services Successfully",
                Status = 200,
                Data = centerServices
            });
        }

        [HttpPost("ReserveAppointment")]
        public IActionResult ReserveAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new ApiResponse<AppointmentDTO> { Status = 400, Message = "Error on reserving Appointment" });


                var appointment = appointmentServices.ReserveAppointment(appointmentDTO);

                return Ok(new ApiResponse<Appointment> { Status = 200, Message = "Appointment reserved successfully.", Data = appointment });
            }
            catch (Exception ex)
            {
                Console.WriteLine("asdasdasd");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest checkoutRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string> { Status = 400, Message = "Invalid checkout request." });

            try
            {
                // Retrieve the appointment
                var appointment = appointmentServices.GetAppointmentById(checkoutRequest.AppointmentId);
                if (appointment == null)
                    return NotFound(new ApiResponse<string> { Status = 404, Message = "Appointment not found." });

                // Check if the appointment is already paid
                if (appointment.AppointmentStatus == AppointmentStatus.Confirmed)
                    return BadRequest(new ApiResponse<string> { Status = 400, Message = "Appointment is already paid." });

                // Validate the client
                if (appointment.UserId != checkoutRequest.ClientId)
                    return BadRequest(new ApiResponse<string> { Status = 400, Message = "Client ID does not match the appointment." });

                // Validate amount
                if (checkoutRequest.Amount <= 0)
                    return BadRequest(new ApiResponse<string> { Status = 400, Message = "Amount must be greater than 0." });

               
                    // Process the payment
                    await appointmentServices.ProcessPayment(checkoutRequest.StripeToken, checkoutRequest.Amount, checkoutRequest.ClientId, checkoutRequest.AppointmentId);




                return Ok(new ApiResponse<string> { Status = 200, Message = "Payment successful. Appointment confirmed." });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string> { Status = 400, Message = $"Payment failed: {ex.Message}" });
            }
        }


        [HttpGet("last-appointment/{userId}")]
        public IActionResult GetLastAppointment(string userId)
        {
            var lastAppointment = appointmentServices.GetLastAppointment(userId);

            if (lastAppointment == null)
                return Ok(new ApiResponse<Appointment> { Status = 400, Message = "No appointments found." });


            return Ok(new ApiResponse<AppointmentDTO> { Status = 200, Message = "Last Appointment retrived.", Data = lastAppointment });

        }

        [HttpGet("upcoming-appointments/{userId}")]
        public IActionResult GetUpcomingAppointments(string userId)
        {
            var upcomings = appointmentServices.GetUpcomingAppointments(userId);

            return Ok(new ApiResponse<List<AppointmentDTO>> { Status = 200, Message = "Last Appointment retrived.", Data = upcomings });

        }

      

        [HttpGet("cards")]
        public IActionResult GetProviderCards()
        {
            var providers = providerServices.GetProviderCards();
            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Cards are displayed.",
                Status = 200,
                Data = providers
            });
        }

        [HttpGet("search")]
        public IActionResult SearchProviders(
           [FromQuery] string? searchText,
           [FromQuery] string? city,
           [FromQuery] string? specialization)
        {
            var providers = providerServices.SearchProviders(searchText, city, specialization);
            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Search Done Successfully",
                Status = 200,
                Data = providers
            });
        }


        [HttpGet("filter-by-day")]
        public IActionResult FilterByDay([FromQuery] DateOnly date)
        {
            var providers = providerServices.FilterByDay(date);

            if (!providers.Any())
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
                Message = $"providers available on {date} retrieved successfully.",
                Status = 200,
                Data = providers
            });
        }

        // Add new review
        [HttpPost("add-review")]
        public IActionResult CreateReview([FromBody] ReviewDTO model)
        {
            if (ModelState.IsValid)
            {
                var review = new Review
                {
                    Rating = model.Rating,
                    Description = model.Description,
                    ProviderId = model.Providerid,
                    ClientId = model.ClientId
                };

                var result = reviewService.CreateReview(review);
                return Ok(new ApiResponse<string>
                {
                    Message = result,
                    Status = 200,
                    Data = result
                });
            }
            return BadRequest(ModelState);
        }

        [HttpGet("provider-reviews")]
        public IActionResult GetReviewsForProvider([FromQuery] string providerId)
        {
            var reviews = reviewService.GetReviewsForProvider(providerId);
            if (!reviews.Any())
            {
                return NotFound(new ApiResponse<List<ReviewByProviderDTO>>
                {
                    Message = "No reviews found",
                    Status = 404,
                    Data = new List<ReviewByProviderDTO>()
                });
            }

            return Ok(new ApiResponse<List<ReviewByProviderDTO>>
            {
                Message = "Reviews retrieved successfully.",
                Status = 200,
                Data = reviews
            });
        }

        [HttpGet("reviews-by-client")]
        public IActionResult GetReviewsByClient([FromQuery] string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return BadRequest(new ApiResponse<string> { Message = "Client ID is required", Status = 400 });

            var reviews = reviewService.GetReviewsForClient(clientId);

            if (!reviews.Any())
                return NotFound(new ApiResponse<string> { Message = "No reviews found for this client", Status = 404 });

            return Ok(new ApiResponse<List<ReviewByClientDTO>>
            {
                Message = "Client reviews retrieved successfully",
                Status = 200,
                Data = reviews
            });
        }









    }
}