using Dorak.DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    //[Authorize(Roles = "Admin , Operator , Provider")]
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
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Failed To Assign Service to Center And Provider!",
                    Status = 400
                });
            }
            var res = S_services.AssignServiceToProviderCenterService(model);
            if (!res)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Data = false,
                    Message = "Failed To Assign Service to Center And Provider!",
                    Status = 400
                });
            }
            return Ok(new ApiResponse<bool>
            {
                Data = res,
                Message = "Assign Service to Center And Provider Successefully ! ",
                Status = 200
            });
        }

    }
}
