using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        private readonly OperatorServices operatorServices;

        public OperatorController(OperatorServices _operatorServices) 
        {
            operatorServices = _operatorServices;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var result = operatorServices.GetAllOperators();
            if (result != null) 
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfull get of operators", Data =  });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "No operators exist" });
        }

        [HttpGet("Delete")]
        public IActionResult Delete(string operatorid)
        {
            var result =  operatorServices.SoftDelete(operatorid);
            if (result == true) {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Deleted." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Something Wrong happened" });
        }

        [HttpGet("Restore")]
        public IActionResult RestoreOperator(string operatorid) 
        {
            var result = operatorServices.RestoreOperator(operatorid);
            if (result == true) 
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Restored." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Successfully Restored." });
        }
    }
}
