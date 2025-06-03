using Dorak.DataTransferObject;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    public class LandingPageController : Controller
    {
        private readonly ProviderServices providerServices;

        public LandingPageController( ProviderServices _providerServices)
        {

            providerServices = _providerServices;
        }
        

            [HttpGet("top-rated")]
        public IActionResult GetTopRatedProviders()
        {

            var topProviders = providerServices.GetTopRatedProviders();

            if (topProviders == null || !topProviders.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = 404,
                    Message = "No top-rated providers found."
                });
            }

            return Ok(new ApiResponse<List<ProviderCardViewModel>>
            {
                Message = "Top-rated provider cards retrieved successfully.",
                Status = 200,
                Data = topProviders
            });
        }

    }
}
