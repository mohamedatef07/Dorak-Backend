using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.Enums;
using Services;
using System.Data.Entity.Core.Common;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CenterController : ControllerBase
    {
        private readonly CenterServices centerServices;
        private readonly OperatorServices operatorServices;
        private readonly ProviderServices providerServices;
        public CenterController(CenterServices _centerServices, OperatorServices _operatorServices, ProviderServices _providerServices)
        {
            centerServices = _centerServices;
            operatorServices = _operatorServices;
            providerServices = _providerServices;
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

        public async Task<IActionResult> AddProvider(RegisterationViewModel user, int centerId, DateOnly startdate, DateOnly enddate, AssignmentType assignmentType)
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

        [HttpPost]
        [Route("UpdateQueueStatus")]
        public IActionResult UpdateQueueStatus([FromBody] UpdateQueueStatusViewModel model)
        {
            var res = operatorServices.UpdateQueueStatus(model);
            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }

        [HttpPost]
        [Route("AssignProviderToCenterManually")]
        public IActionResult AssignProvider([FromBody] ProviderAssignmentViewModel model)
        {

            var res = providerServices.AssignProviderToCenter(model);

            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }

        [HttpPost]
        [Route("AssignProviderToCenterWeekly")]
        public IActionResult AssignWeekly([FromBody] WeeklyProviderAssignmentViewModel model)
        {
            var res = providerServices.AssignProviderToCenterWithWorkingDays(model);
            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }

        [HttpPost]
        [Route("RescheduleAssignment")]
        public IActionResult RescheduleAssignment([FromBody] RescheduleAssignmentViewModel model)
        {
            var res = providerServices.RescheduleAssignment(model);
            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }


        [HttpPost]
        [Route("AssignServiceToCenter")]
        public IActionResult AssignServiceToCenter([FromBody] AssignProviderCenterServiceViewModel model)
        {

            var res = providerServices.AssignServiceToCenter(model);
            return Ok(new ApiResponse<string>
            {
                Data = res,
                Message = "Success",
                Status = 200
            });
        }

    }
}
