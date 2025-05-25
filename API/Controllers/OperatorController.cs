using Dorak.DataTransferObject;
using Dorak.ViewModels;
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
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfull get of operators"});
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
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Unsuccessfully Restored." });
        }

        [HttpGet("Startshift")]
        public IActionResult StartShift(int shiftId, string operatorid) 
        {
            var result = operatorServices.StartShift(shiftId, operatorid);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully Started." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Failed to Start." });
        }

        [HttpGet("EndShift")]
        public IActionResult EndShift(int shiftId, string operatorid)
        {
            var result = operatorServices.EndShift(shiftId, operatorid);
            if (result == true)
            {
                return Ok(new ApiResponse<OperatorViewModel> { Status = 200, Message = "Successfully End Shift." });
            }
            return Ok(new ApiResponse<OperatorViewModel> { Status = 400, Message = "Failed to End Shift." });
        }
    }
}
