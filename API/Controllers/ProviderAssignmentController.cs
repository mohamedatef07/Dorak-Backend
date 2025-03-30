using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/{Controller}")]
    [ApiController]
    public class ProviderAssignmentController : ControllerBase
    {
        ProviderServices providerServices;

        public ProviderAssignmentController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }

        
        [HttpGet("provider/{providerId}")]
        public async Task<IActionResult> GetProviderById(string providerId)
        {
            var provider = await providerServices.GetProviderByIdAsync(providerId);

            return Ok(provider);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignProvider(
            string providerId,
            int centerId,
            DateTime startDate,
            DateTime endDate,
            ProviderType assignmentType)
        {
            await providerServices.AssignProviderToCenterAsync(
                providerId,
                centerId,
                startDate,
                endDate,
                assignmentType
            );

            return Ok("Provider assigned successfully.");
        }
    }
}
