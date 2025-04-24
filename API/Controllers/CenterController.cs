using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Services;
using System.Threading.Tasks;

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
                Status = 200
            });
        }


        [HttpPost]  //provider or assignment !!!!!!!! Type
        [Route("addProviderAndAssignIt")]

        public async Task<IActionResult> AddProvider(RegisterationViewModel user, int centerId, DateTime startdate, DateTime enddate, AssignmentType assignmentType)
        {
            var res = await centerServices.AddProviderAsync(user, centerId, startdate, enddate, assignmentType);

            if (res.Succeeded)
            {
                return Ok("Provider created and assigned to center successfully!");
            }
            else
            {
                return BadRequest(res.Errors);
            }
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
