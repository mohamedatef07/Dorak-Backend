using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        public ProviderServices providerServices;
        public ProviderCardService providerCardService;
        public ShiftServices shiftServices;
        private readonly AppointmentServices _appointmentServices;

        public ClientController(AppointmentServices appointmentServices, ProviderServices _providerServices, ProviderCardService providerCardService, ShiftServices _shiftServices)
        {
            providerServices = _providerServices;
            this.providerCardService = providerCardService;
            shiftServices = _shiftServices;
            _appointmentServices = appointmentServices;

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
        public async Task<IActionResult> ReserveAppointment([FromBody] ReserveAppointmentRequest reserveAppointmentRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(new ApiResponse<AppointmentDTO> { Status = 400, Message = "Error on reserving Appointment" });


                var appointment = await _appointmentServices.ReserveAppointment(reserveAppointmentRequest.AppointmentDTO, reserveAppointmentRequest.StripeToken, reserveAppointmentRequest.Amount, reserveAppointmentRequest.AppointmentDTO.UserId);

                return Ok(new ApiResponse<Appointment> { Status = 200, Message = "Appointment reserved successfully.", Data = appointment });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("last-appointment/{userId}")]
        public IActionResult GetLastAppointment(string userId)
        {
            var lastAppointment = _appointmentServices.GetLastAppointment(userId);

            if (lastAppointment == null)
                return Ok(new ApiResponse<Appointment> { Status = 400, Message = "No appointments found." });


            return Ok(new ApiResponse<AppointmentDTO> { Status = 200, Message = "Last Appointment retrived.", Data = lastAppointment });

        }

        [HttpGet("upcoming-appointments/{userId}")]
        public IActionResult GetUpcomingAppointments(string userId)
        {
            var upcomings = _appointmentServices.GetUpcomingAppointments(userId);

            return Ok(new ApiResponse<List<AppointmentDTO>> { Status = 200, Message = "Last Appointment retrived.", Data = upcomings });

        }


    }
}