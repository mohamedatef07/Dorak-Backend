using Dorak.DataTransferObject;
using Dorak.DataTransferObject.AccountDTO;
using Dorak.Models;
using Dorak.ViewModels;
using Dorak.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.EmailService;
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
        private readonly IEmailSender _emailSender;
        public AccountController(AccountServices accountServices,
                                ClientServices clientServices,
                                ProviderServices providerServices,
                                OperatorServices operatorServices,
                                AdminCenterServices adminCenterServices,
                                UserManager<User> userManager,
                                IEmailSender emailSender)
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
        public async Task<IActionResult> Register([FromForm] RegisterationViewModel user)
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

        [HttpPost("ClientRegister")]
        public async Task<IActionResult> ClientRegister(NewClientViewModel client)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage);
                return BadRequest(new ApiResponse<object> { Message = string.Join(" ", errors), Status = 400 });
            }

            var result = await _clientServices.ClientRegister(client);
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

            var loginResult = await _accountServices.LoginWithGenerateJWTToken(user);

            if (!loginResult.Succeeded)
            {
                var lowerMessage = loginResult.Message.ToLower();

                if (lowerMessage.Contains("not found"))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Message = loginResult.Message,
                        Status = 404,
                        Data = null
                    });
                }

                if (lowerMessage.Contains("locked") || lowerMessage.Contains("disabled"))
                {
                    return StatusCode(403, new ApiResponse<object>
                    {
                        Message = loginResult.Message,
                        Status = 403,
                        Data = null
                    });
                }

                return Unauthorized(new ApiResponse<object>
                {
                    Message = loginResult.Message,
                    Status = 401,
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Message = "Logged In Successfully",
                Status = 200,
                Data = new
                {
                    Token = loginResult.Token,
                    RefreshToken = loginResult.RefreshToken,
                    Roles = loginResult.Roles
                }
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
                Console.WriteLine("⛔ Invalid model state.");
                return BadRequest(new ApiResponse<object> { Message = "Invalid Email", Status = 400 });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                Console.WriteLine($"⚠️ No user found with email: {model.Email} or email not confirmed.");
                return Ok(new ApiResponse<string>
                {
                    Message = "If that email exists, a reset link has been sent",
                    Status = 200
                });
            }

            // Generate token and encode it
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);

            // Build reset link
            var resetLink = $"{model.ClientAppUrl}/reset-password?email={user.Email}&token={encodedToken}";

            Console.WriteLine($"✅ Sending reset link to: {user.Email}");
            Console.WriteLine($"🔗 Reset Link: {resetLink}");

            // Create email body
            string htmlBody = $@"
            <p>Hello {user.UserName},</p>
            <p>You requested to reset your password.</p>
            <p>Click the button below to set a new password:</p>
            <p><a href='{resetLink}' style='padding:10px 20px;background:#007BFF;color:#fff;text-decoration:none;border-radius:5px;'>Reset Password</a></p>
            <p>If you didn’t request this, you can ignore this email.</p>
            <p>Thanks,<br/>Dorak Team</p>";

            try
            {
                await _emailSender.SendEmailAsync(user.Email, "Reset Your Password", htmlBody);
                Console.WriteLine("📨 Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Failed to send email:");
                Console.WriteLine(ex.Message);
                return StatusCode(500, new
                {
                    error = "Failed to send email",
                    details = ex.Message
                });
            }

            return Ok(new ApiResponse<string>
            {
                Message = "If that email exists, a reset link has been sent",
                Status = 200
            });
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