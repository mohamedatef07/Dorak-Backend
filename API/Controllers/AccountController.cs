using Dorak.DataTransferObject;
using Dorak.DataTransferObject.AccountDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;
using System.Web;

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
        private readonly UserManager<User> _userManager;
        private readonly MailKitEmailSender _emailSender;

        public AccountController(AccountServices accountServices,
                                ClientServices clientServices,
                                ProviderServices providerServices,
                                OperatorServices operatorServices,
                                AdminCenterServices adminCenterServices,
                                UserManager<User> userManager,
                                MailKitEmailSender emailSender)
        {
            _accountServices = accountServices;
            _clientServices = clientServices;
            _providerService = providerServices;
            _operatorServices = operatorServices;
            _adminCenterServices = adminCenterServices;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm]RegisterationViewModel user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _accountServices.CreateAccount(user);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object> { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Account creation failed", Status = 400 });
        }

        #region Creating Users
        [HttpPost("CreateClient")]
        public async Task<IActionResult> CreateClient(string id, ClientRegisterViewModel client)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _clientServices.CreateClient(id, client);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object> { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateProvider")]
        public async Task<IActionResult> CreateProvider(string id, ProviderRegisterViewModel provider)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _providerService.CreateProvider(id, provider);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object> { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateOperator")]
        public async Task<IActionResult> CreateOperator(string id, OperatorViewModel _operator)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _operatorServices.CreateOperator(id, _operator);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object> { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Client creation failed", Status = 400 });
        }

        [HttpPost("CreateAdminCenter")]
        public async Task<IActionResult> CreateAdminCenter(string id, AdminCenterViewModel adminCenter)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _adminCenterServices.CreateAdminCenter(id, adminCenter);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object> { Message = "Your Account Added Successfully, Go to Login", Status = 200 });
            }

            return BadRequest(new ApiResponse<object> { Message = "Client creation failed", Status = 400 });
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
            return Ok(new ApiResponse<object> { Message = "Sign out Successfully", Status = 200 });
        }

        //change password
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Invalid data",
                    Status = 400,
                    Data = null
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Message = "Unauthorized",
                    Status = 401,
                    Data = null
                });
            }

            var result = await _accountServices.ChangePasswordAsync(userId, model);

            if (result.StartsWith("Password changed"))
            {
                return Ok(new ApiResponse<object>
                {
                    Message = result,
                    Status = 200,
                    Data = null
                });
            }

            return BadRequest(new ApiResponse<object>
            {
                Message = result,
                Status = 400,
                Data = null
            });
        }

        //Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Email", Status = 400 });
            }
            User? user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return Ok(new ApiResponse<object> { Message = "If that email exists, a reset link has been sent", Status = 200 });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            var resetLink = $"{model.ClientAppUrl}/reset-password?email={user.Email}&token={encodedToken}";

            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Click the link to reset your password: <a href='{resetLink}'>Reset Password</a>");
            return Ok(new ApiResponse<string> { Message = "If that email exists, a reset link has been sent", Status = 200 });
        }
        //Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object> { Message = "Invalid Data", Status = 400 });
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse<object> { Message = "User not found", Status = 404 });
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiResponse<object> { Message = errors, Status = 400 });
            }
            return Ok(new ApiResponse<object> { Message = "Password has been reset successfully", Status = 200 });
        }

    }
}