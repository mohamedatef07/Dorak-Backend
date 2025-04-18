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
        private readonly AppointmentServices _appointmentServices;

        public ClientController(AppointmentServices appointmentServices)
        {
            _appointmentServices = appointmentServices;
        }


        [HttpPost("ReserveAppointment")]
        public IActionResult ReserveAppointment([FromBody] AppointmentDTO appointmentDTO)
        {
            if (!ModelState.IsValid)

                return Ok(new ApiResponse<AppointmentDTO> { Status = 400, Message = "Error on reserving Appointment" });

            var appointment = _appointmentServices.ReserveAppointment(appointmentDTO);

            return Ok(new ApiResponse<Appointment> { Status = 200, Message = "Appointment reserved successfully.", Data = appointment });

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
