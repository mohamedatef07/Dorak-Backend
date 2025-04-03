using Microsoft.AspNetCore.Mvc;
using Services;
using Models.Enums;
using Dorak.ViewModels;
using System;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderAssignmentController : ControllerBase
    {
        private readonly ProviderServices providerServices;

        public ProviderAssignmentController(ProviderServices _providerServices)
        {
            providerServices = _providerServices;
        }

        [HttpGet]
        [Route("getproviderbyid")]
        public IActionResult GetProviderById(string providerId)
        {
            var provider = providerServices.GetProviderById(providerId);
            return Ok(provider);
        }

        [HttpPost]
        [Route("assign")]
        public IActionResult AssignProvider(ProviderAssignmentViewModel model)
        {

            providerServices.AssignProviderToCenter(
                model
            );

            return Ok("provider assigned successfully.");
        }
    }
}
