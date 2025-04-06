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
        //public IActionResult Index()
        //{
        //    return JsonResult(new {});
        //}
        [HttpGet("ProviderInfo")]
        public IActionResult ProviderMainInfo(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Message = "Provider ID is required", Status = 400 });
            }

            var provider = providerServices.GetProviderById(id);

            if (provider == null)
            {
                return NotFound(new { Message = "Provider not found", Status = 404 });
            }
            var providerVM = new GetProviderMainInfo
            {
                FirstName = provider.FirstName,
                LastName = provider.LastName,
                Specialization = provider.Specialization,
                Bio = provider.Bio,
                Rate = provider.Rate,
                Image = provider.Image
            };
            return Ok(new ApiResponse<GetProviderMainInfo>
            {
                Message = "Get Provider Info Successfully",
                Status = 200,
                Data = providerVM
            });
        }

    }
}
