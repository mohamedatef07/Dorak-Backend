using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CenterController : ControllerBase
    {
        private readonly CenterServices centerServices;

        public CenterController(CenterServices _centerServices)
        {
            centerServices = _centerServices;
        }
        [HttpGet]
        [Route("AllProviders")]
        public IActionResult AllProvider(int CenterId)
        {

            var res = centerServices.GetProvidersOfCenter(CenterId);
            return Ok(res);
        }


        [HttpPost]
        [Route("AddProvider")]
        public IActionResult AddProvider(RegisterationViewModel user)
        {
            var res =  centerServices.AddProviderAsync(user);
            return Ok(res);
        }
    }
}
