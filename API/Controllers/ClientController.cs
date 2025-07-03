using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ClientDTO;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Services;
using System.Security.Claims;



namespace API.Controllers
{
    [Authorize(Roles = "Client")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ProviderServices providerServices;
        private readonly ShiftServices shiftServices;
        private readonly AppointmentServices appointmentServices;
        private readonly ReviewServices reviewServices;
        private readonly ClientServices clientServices;
        private readonly LiveQueueServices liveQueueServices;

        public ClientController(AppointmentServices _appointmentServices, LiveQueueServices _liveQueueServices, ProviderServices _providerServices, ShiftServices _shiftServices, ReviewServices _reviewServices, ClientServices _clientServices)
        {
            providerServices = _providerServices;
            shiftServices = _shiftServices;
            appointmentServices = _appointmentServices;
            reviewServices = _reviewServices;
            clientServices = _clientServices;
            liveQueueServices = _liveQueueServices;
        }
        [HttpGet("main-info")]
        public IActionResult ProviderMainInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<object> { Message = "Provider id is required", Status = 400 });
            }
            var provider = providerServices.GetProviderById(providerId);

            if (provider == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
            }
            GetProviderMainInfoDTO mainInfo = providerServices.GetProviderMainInfo(provider);
            return Ok(new ApiResponse<GetProviderMainInfoDTO>
            {
                Message = "Get Provider Info Successfully",
                Status = 200,
                Data = mainInfo
            });
        }
        [HttpGet("booking-info")]
        public IActionResult ProviderBookingInfo([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<object> { Message = "Provider id is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderBookingInfoDTO> providerBookingInfo = providerServices.GetProviderBookingInfo(provider);
            if (providerBookingInfo == null || !providerBookingInfo.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "Provider booking info not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderBookingInfoDTO>>
            {
                Message = "Get Provider  Booking Info Successfully",
                Status = 200,
                Data = providerBookingInfo
            });
        }

        [HttpGet("provider-center-services")]
        public IActionResult ProviderCenterServices([FromQuery] string providerId)
        {
            if (string.IsNullOrWhiteSpace(providerId))
            {
                return BadRequest(new ApiResponse<object> { Message = "Provider id is required", Status = 400 });
            }
            Provider provider = providerServices.GetProviderById(providerId);
            if (provider == null)
            {
                return NotFound(new ApiResponse<object> { Message = "Provider not found", Status = 404 });
            }
            List<GetProviderCenterServicesDTO> centerServices = providerServices.GetCenterServices(provider);
            if (centerServices == null || !centerServices.Any())
            {
                return NotFound(new ApiResponse<object> { Message = "Provider center services not found", Status = 404 });
            }
            return Ok(new ApiResponse<List<GetProviderCenterServicesDTO>>
            {
                Message = "Get provider center services Successfully",
                Status = 200,
                Data = centerServices
            });
        }

        [HttpPost("reserve-appointment")]
        public async Task<IActionResult> ReserveAppointment([FromBody] ReserveApointmentDTO reserveApointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Status = 400, Message = "Invalid appointment data" });
            }
            var checkoutReq = await appointmentServices.ReserveAppointment(reserveApointmentDTO);
            if (checkoutReq == null)
            {
                return BadRequest(new ApiResponse<object> { Status = 404, Message = "Appointment not found or already reserved" });
            }
            return Ok(new ApiResponse<CheckoutRequest> { Status = 200, Message = "Appointment reserved successfully", Data = checkoutReq });
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
                return NotFound(new ApiResponse<Appointment> { Status = 400, Message = "No appointments found." });


            return Ok(new ApiResponse<AppointmentDTO> { Status = 200, Message = "Last Appointment retrived.", Data = lastAppointment });

        }

        [HttpGet("upcoming-appointments/{userId}")]
        public IActionResult GetUpcomingAppointments(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var PaginationResponse = appointmentServices.GetUpcomingAppointments(userId, pageNumber, pageSize);
            if (PaginationResponse.Data == null || !PaginationResponse.Data.Any())
            {
                return BadRequest(new PaginationApiResponse<object>(false, "No found upcoming appointments", 400, null, 0, pageNumber, pageSize));
            }
            return Ok(PaginationResponse);
        }

        [HttpGet("cards")]
        public IActionResult GetProviderCards()
        {
            var providers = providerServices.GetProviderCards();
            if (providers == null || !providers.Any())
            {
                return BadRequest(new ApiResponse<object> { Status = 404, Message = "No found providers" });
            }
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
            if (providers == null || !providers.Any())
            {
                return BadRequest(new ApiResponse<object> { Status = 404, Message = "No found providers" });
            }
            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Search Done Successfully",
                Status = 200,
                Data = providers
            });
        }


        [HttpPost("filter")]
        public IActionResult FilterDoctors([FromBody] FilterProviderDTO filter)
        {
            var result = providerServices.FilterProviders(filter);

            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Status = 200,
                Message = "Filtered Successfully",
                Data = result
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

                var result = reviewServices.CreateReview(review);
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
            var reviews = reviewServices.GetReviewsForProvider(providerId);
            if (!reviews.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "No reviews found",
                    Status = 404
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

            var reviews = reviewServices.GetReviewsForClient(clientId);

            if (!reviews.Any())
                return NotFound(new ApiResponse<string> { Message = "No reviews found for this client", Status = 404 });

            return Ok(new ApiResponse<List<ReviewByClientDTO>>
            {
                Message = "Client reviews retrieved successfully",
                Status = 200,
                Data = reviews
            });
        }

        [HttpGet("profile-all-appointment/{userId}")]
        public IActionResult ProfileAndAllAppointments(string userId)
        {
            var profile = clientServices.GetProfile(userId);
            if (profile == null)
            {
                return Ok(new ApiResponse<object> { Status = 404, Message = "No found appointments" });
            }
            return Ok(new ApiResponse<ClientProfileDTO> { Status = 200, Message = "Profile retrived.", Data = profile });
        }


        [HttpGet("profile-for-live-queue/{userId}")]
        public IActionResult ProfileForliveQueue(string userId)
        {
            var profile = clientServices.GetClientInfoToLiveQueue(userId);
            if (profile == null)
            {
                return Ok(new ApiResponse<object> { Status = 404, Message = "Not found " });
            }
            return Ok(new ApiResponse<ClientInfoToLiveQueueDTO> { Status = 200, Message = "Profile retrived.", Data = profile });
        }



        [HttpGet("client-wallet/{userId}")]
        public IActionResult ClientWalletAndProfile(string userId)
        {
            var clientWalletProfileDTO = clientServices.GetClientWalletAndProfile(userId);
            if (clientWalletProfileDTO == null)
            {
                return Ok(new ApiResponse<object> { Status = 404, Message = "No found wallet" });
            }
            return Ok(new ApiResponse<ClientWalletAndProfileDTO> { Status = 200, Message = "wallet Retrive", Data = clientWalletProfileDTO });
        }



        //live queue NT
        //[HttpGet("shift-queue/{shiftId}/user/{userId}")]
        //public async Task<IActionResult> GetShiftQueueWithClientFlag(int shiftId, string userId)
        //{
        //    var queue = await liveQueueServices.GetLiveQueueForShiftAsync(shiftId, userId);

        //    if (queue == null || !queue.Any())
        //    {
        //        return Ok(new ApiResponse<object> { Status = 404, Message = "No queue data found." });
        //    }

        //    return Ok(new ApiResponse<List<ClientLiveQueueDTO>>
        //    {
        //        Status = 200,
        //        Message = "Queue retrieved successfully.",
        //        Data = queue
        //    });
        //}



        [HttpGet("queue/by-appointment/{appointmentId}")]
        public IActionResult GetQueueByAppointment(int appointmentId)
        {


            var result = liveQueueServices.GetQueueEntryByAppointmentId(appointmentId);
            if (result == null || !result.Any())
                return NotFound("No live queue found for this appointment.");

            return Ok(new ApiResponse<List<ClientLiveQueueDTO>>
            {
                Status = 200,
                Message = "Queue retrieved successfully.",
                Data = result
            });
        }


        [HttpGet("appointment/{appointmentid}")]
        public IActionResult GetAppointmentById(int appointmentid)
        {

            if (appointmentid <= 0)
                return BadRequest(new ApiResponse<object> { Status = 400, Message = "Invalid appointment" });

            var Appointment = appointmentServices.GetAppointmentbyId(appointmentid);

            if (Appointment == null)
                return NotFound(new ApiResponse<object> { Status = 400, Message = "No appointments found." });


            return Ok(new ApiResponse<AppointmentDTO> { Status = 200, Message = "Appointment retrived.", Data = Appointment });

        }


        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateClientProfileAsync([FromForm] UpdateClientProfileDTO updateClientProfile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new ApiResponse<object> { Message = "User id is required", Status = 400 });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Data", Status = 500 });
            }

            var result = await clientServices.UpdateClientProfile(userId, updateClientProfile);

            if (!result)
            {
                return BadRequest(new ApiResponse<object> { Status = 400, Message = "Not Updated" });
            }
            return Ok(new ApiResponse<bool> { Status = 200, Message = "Profile Update", Data = true });
        }

        [HttpGet("ClientProfile")]
        public IActionResult GetMyClientProfile()
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

            var profile = clientServices.GetCLientProfileForUpdate(userId);

            if (profile == null)
            {
                return NotFound(new ApiResponse<string>
                {
                    Message = "Client not found",
                    Status = 404,
                    Data = null
                });
            }

            return Ok(new ApiResponse<ClientDetailsDTO>
            {
                Message = "Client profile retrieved successfully",
                Status = 200,
                Data = profile
            });
        }

        [HttpGet("client-general-appointment-statistics")]
        public IActionResult GeneralAppointmentStatistics()
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(clientId))
            {
                return BadRequest(new ApiResponse<object> { Message = "User id is required", Status = 400 });
            }
            var appoinmentStatistics = clientServices.GetGeneralAppoinmentStatistics(clientId);
            if (appoinmentStatistics == null)
            {
                return NotFound(new ApiResponse<object> { Message = "General appoinment statistics not found", Status = 404 });
            }
            return Ok(new ApiResponse<GetClientAppointmentStatisticsDTO> { Message = "Get general appoinment statistics successfully", Status = 200, Data = appoinmentStatistics });
        }
        [HttpPost("cancel-appointment")]
        public async Task<IActionResult> CancelAppointment([FromQuery] int appointmentId)
        {
            if (appointmentId <= 0)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid appointment id", Status = 400 });
            }
            var isCancelled = await appointmentServices.clientCancelAppointment(appointmentId);
            if (!isCancelled)
            {
                return BadRequest(new ApiResponse<object> { Message = "Can't cancel the appointment", Status = 400 });
            }
            return Ok(new ApiResponse<object> { Message = "Appointment cancelled successfully", Status = 200 });
        }

        [HttpGet("appointments-history/{userId}")]
        public IActionResult GetAppointmentsHistory(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var PaginationResponse = appointmentServices.GetAppointmentsHistory(userId, pageNumber, pageSize);
            if (PaginationResponse.Data == null || !PaginationResponse.Data.Any())
            {
                return BadRequest(new PaginationApiResponse<object>(false, "No found History appointments", 400, null, 0, pageNumber, pageSize));
            }
            return Ok(PaginationResponse);
        }
    }
}