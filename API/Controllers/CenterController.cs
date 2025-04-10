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
            return Ok(new ApiResponse<PaginationViewModel<ProviderViewModel>>
            {
                Data = res,
                Message = "Success",
                Status=200
            });
        }


        [HttpPost]
        [Route("AddProvider")]
        public IActionResult AddProvider(RegisterationViewModel user)
        {
            var res =  centerServices.AddProviderAsync(user);
            return Ok(res);
        }

        [HttpPost]
        [Route("DeleteProvider")]
        public IActionResult DeleteProviderFromCenter(string ProviderId)
        {
            var res = centerServices.DeleteProviderfromCenter(ProviderId);
            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }


        [HttpGet]
        [Route("SearchProvider")]
        public IActionResult SearchProvider(int centerId, string searchText = "", int pageNumber = 1,
                                                            int pageSize = 2)
        {
            var res = centerServices.ProviderSearch(centerId, searchText, pageNumber, pageSize);
            return Ok(new ApiResponse<PaginationViewModel<ProviderViewModel>> { Data = res, Message = "Success", Status = 200 });
        }

    }
}
