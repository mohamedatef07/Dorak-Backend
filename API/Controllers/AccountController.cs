using Dorak.DataTransferObject;
using Dorak.DataTransferObject.ProviderDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountServices _accountServices;
        private readonly ClientServices _clientServices;
        private readonly ProviderServices _providerService;
        private readonly OperatorServices _operatorServices;
        private readonly AdminCenterServices _adminCenterServices;

        public AccountController(AccountServices accountServices,
                                ClientServices clientServices,
                                ProviderServices providerServices,
                                OperatorServices operatorServices,
                                AdminCenterServices adminCenterServices)
        {
            _accountServices = accountServices;
            _clientServices = clientServices;
            _providerService = providerServices;
            _operatorServices = operatorServices;
            _adminCenterServices = adminCenterServices;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterationViewModel user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _accountServices.CreateAccount(user);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new { Message = "Account creation failed", Status = 400 });
        }

        #region Creating Users
        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient(string id, ClientRegisterViewModel client)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _clientServices.CreateClient(id, client);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateProvider")]
        public async Task<IActionResult> CreateProvider(string id, ProviderRegisterViewModel provider)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _providerService.CreateProvider(id, provider);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateOperator")]
        public async Task<IActionResult> CreateOperator(string id, OperatorViewModel _operator)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _operatorServices.CreateOperator(id, _operator);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateAdminCenter")]
        public async Task<IActionResult> CreateAdminCenter(string id, AdminCenterViewModel adminCenter)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _adminCenterServices.CreateAdminCenter(id, adminCenter);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new { Message = "Client creation failed", Status = 400 });
        }

        #endregion

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginViewModel user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object>
                {
                    Message = string.Join(" ", errors),
                    Status = 400,
                    Data = null
                });
            }

            var (token, refreshToken) = await _accountServices.LoginWithGenerateJWTToken(user);

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Message = "Invalid Email, Username, or Password",
                    Status = 401,
                    Data = null
                });
            }

            var roles = await _accountServices.GetUserRolesAsync(user.UserName);

            var responseData = new
            {
                Token = token,
                RefreshToken = refreshToken,
                Roles = roles
            };

            return Ok(new ApiResponse<object>
            {
                Message = "Logged In Successfully",
                Status = 200,
                Data = responseData
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<string>
                {
                    Message = string.Join(" ", errors),
                    Status = 400,
                    Data = null
                });
            }

            var result = await _accountServices.RefreshTokenAsync(request);
            if (result.NewAccessToken == null)
            {
                return Unauthorized(new ApiResponse<string>
                {
                    Message = "Invalid or expired refresh token.",
                    Status = 401,
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Token refreshed successfully.",
                Status = 200,
                Data = new
                {
                    AccessToken = result.NewAccessToken,
                    RefreshToken = result.NewRefreshToken
                }
            });
        }


        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await _accountServices.Signout();
            return Ok(new { Message = "Sign out Successfully", Status = 200 });
        }

        //change password
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var result = await _accountServices.ChangePasswordAsync(model);

            return Ok(new ApiResponse<string>
            {
                Message = result,
                Status = 200,
                Data = result
            });
        }
    }
}
