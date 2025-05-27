using Dorak.DataTransferObject;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;


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

        [HttpPost]
        [Route("AddProviderAndAssignIt")]
        public async Task<IActionResult> AddProviderAndAssignIt([FromForm]ProviderAssignmentDTO providerDto)
        {

            var res = await centerServices.GetProviderID(providerDto);
            return Ok(new ApiResponse<string>

            {
                Data = res,
                Message = "Success",
                Status = 200
            });


        }


        //[HttpGet]
        //[Route("AllProviders")]
        //public IActionResult AllProvider(int CenterId)
        //{

        //    var res = centerServices.GetProvidersOfCenter(CenterId);
        //    return Ok(new ApiResponse<PaginationViewModel<ProviderViewModel>>
        //    {
        //        Data = res,
        //        Message = "Success",
        //        Status = 200
        //    });
        //}

        [HttpGet]
        [Route("AllProviders")]
        public IActionResult AllProviders(int centerId, int pageNumber = 1, int pageSize = 9, string specializationFilter = "")
        {
            try
            {
                var res = centerServices.GetProvidersOfCenter(centerId, pageNumber, pageSize, "StartDate", specializationFilter);
                return Ok(new ApiResponse<PaginationViewModel<ProviderViewModel>> { Data = res, Message = "Success", Status = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { Message = $"Internal Server Error: {ex.Message}", Status = 500 });
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
        public IActionResult SearchProvider(string searchText = "", int pageNumber = 1, int pageSize = 9, string sortBy = "", string specializationFilter = "")
        {
            try
            {
                var res = centerServices.ProviderSearch(searchText, pageNumber, pageSize, specializationFilter);
                return Ok(new ApiResponse<PaginationViewModel<ProviderViewModel>> { Data = res, Message = "Success", Status = 200 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchProvider: {ex.Message}");
                return StatusCode(500, new ApiResponse<string> { Data = null, Message = "Internal Server Error", Status = 500 });
            }
        }

        // [Authorize(Roles = "Admin, Operator")]
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

        [Authorize(Roles = "Admin, Operator")]
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

        [Authorize(Roles = "Operator")]
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

        [Authorize(Roles = "Admin, Operator")]
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
