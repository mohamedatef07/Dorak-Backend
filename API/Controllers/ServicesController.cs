using Dorak.DataTransferObject;
using Dorak.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Authorize(Roles = "Admin , Operator , Provider")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly S_Services S_services;

        public ServicesController(S_Services _Services)
        {
            S_services = _Services;
        }

        [HttpGet]
        [Route("get-all-services")]
        public IActionResult GetAllServices()
        {
            var services = S_services.GetAllServiceDropDown(); // Calling the service method to get all services

            if (services == null || services.Count == 0)
            {
                return NotFound(new ApiResponse<string>
                {
                    Data = "No services found.",
                    Message = "Failure",
                    Status = 404
                });
            }

            return Ok(new ApiResponse<List<ServicesDTO>>
            {
                Data = services,
                Message = "Retrive Services Successfully !",
                Status = 200
            });
        }

        [HttpPost]
        [Route("assign-service-to-provider-center-service")]
        public IActionResult AssignServiceToProviderCenterService([FromBody] AddProviderCenterServiceDTO model)
        {
            if (!ModelState.IsValid)
            {
                // ModelState.IsValid handles validation attributes on your DTO (e.g., [Required])
                // This is a good first line of defense for basic validation.
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Invalid request data. Please check your input.",
                    Status = 400
                });
            }

            var result = S_services.AssignServiceToProviderCenterService(model);

            switch (result)
            {
                case AssignServiceResult.Success:
                    return Ok(new ApiResponse<bool>
                    {
                        Data = true,
                        Message = "Service assigned to center and provider successfully!",
                        Status = 200
                    });

                case AssignServiceResult.InvalidInput:
                    return BadRequest(new ApiResponse<bool>
                    {
                        Data = false,
                        Message = "Missing or invalid required fields (ProviderId, ServiceId, CenterId, or Price).",
                        Status = 400
                    });

                case AssignServiceResult.ProviderNotFound:
                    return NotFound(new ApiResponse<bool> // Using NotFound (404) for entity not found
                    {
                        Data = false,
                        Message = "The specified provider was not found.",
                        Status = 404
                    });

                case AssignServiceResult.ServiceNotFound:
                    return NotFound(new ApiResponse<bool>
                    {
                        Data = false,
                        Message = "The specified service was not found.",
                        Status = 404
                    });

                case AssignServiceResult.CenterNotFound:
                    return NotFound(new ApiResponse<bool>
                    {
                        Data = false,
                        Message = "The specified center was not found.",
                        Status = 404
                    });

                case AssignServiceResult.AssignmentAlreadyExists:
                    return Conflict(new ApiResponse<bool> // Using Conflict (409) for duplicate resource
                    {
                        Data = false,
                        Message = "This service is already assigned to the specified provider and center.",
                        Status = 409
                    });

                case AssignServiceResult.UnknownError:
                default: // Catch any unhandled or new enum values
                    return StatusCode(500, new ApiResponse<bool> // Internal Server Error for unexpected issues
                    {
                        Data = false,
                        Message = "An unexpected error occurred while assigning the service. Please try again later.",
                        Status = 500
                    });
            }
        }

    }
}
