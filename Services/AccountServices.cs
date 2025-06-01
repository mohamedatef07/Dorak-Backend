using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dorak.Models;
using Dorak.ViewModels;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Repositories;
using Models.Enums;
using System.Security.Cryptography;
using Data;
using Dorak.DataTransferObject;
using Microsoft.AspNetCore.Hosting;



namespace Services
{
    public class AccountServices
    {
        public AccountRepository accountRepository;
        public ClientServices clientServices;
        public ProviderServices providerServices;
        public OperatorServices operatorServices;
        public ProviderRepository providerRepository;
        public OperatorRepository operatorRepository;
        public AdminCenterManagement adminCenterManagement;
        private UserManager<User> userManager;
        public CommitData CommitData;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration configuration;
        public AccountServices(AccountRepository _AccountRepository,
                               ClientServices _clientServices,
                               ProviderServices _providerServices,
                               OperatorServices _operatorServices,
                               ProviderRepository _ProviderRepository,
                               IConfiguration _configuration,
                               AdminCenterManagement _adminCenterManagement,
                               CommitData _commitData,
                               UserManager<User> _userManager,
                               IWebHostEnvironment env)
        {
            accountRepository = _AccountRepository;
            clientServices = _clientServices;
            providerServices = _providerServices;
            operatorServices = _operatorServices;
            providerRepository = _ProviderRepository;
            configuration = _configuration;
            adminCenterManagement = _adminCenterManagement;
            CommitData = _commitData;
            userManager = _userManager;
            _env = env;
        }



        #region Helper
        private async Task<string> SaveImageAsync(RegisterationViewModel user)
        {
            var currentUser = accountRepository.FindByUserName(user.UserName);
            if (user.Image != null && user.Image.Length > 0)
            {
                var folderPath = Path.Combine(_env.WebRootPath, "image", $"{user.Role}", currentUser.Id.ToString());
                Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(user.Image.FileName);
                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await user.Image.CopyToAsync(stream);
                }

                string ImagePath = $"/image/{user.Role}/{currentUser.Id}/{fileName}";

                return ImagePath;

            }
            return null;
        }
        #endregion

        public async Task<string> getIdByUserName(string username)
        {
            var user = await accountRepository.FindByUserName(username);
            return user?.Id;
        }

        public async Task<IdentityResult> CreateAccount(RegisterationViewModel user)
          {
            var userRes = await accountRepository.Register(user);

            if (userRes.Succeeded)
            {
                // Our Roles:
                //SuperAdmin / Admin (Center Admin) / Operator / Provider / Client
                var currentUser = await accountRepository.FindByUserName(user.UserName);
                if (user.Role == "Admin")
                {
                    //................
                }
                else if (user.Role == "Operator")
                {
                    var operarorres = await operatorServices.CreateOperator(currentUser.Id, new OperatorViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Gender = user.Gender,
                        Image = await SaveImageAsync(user),
                        CenterId = user.CenterId
                    });
                    if (operarorres.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                }
                else if (user.Role == "Provider")
                {

                    var providerres = await providerServices.CreateProvider(currentUser.Id, new ProviderRegisterViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Specialization = user.Specialization,
                        Bio = user.Bio,
                        ExperienceYears = user.ExperienceYears,
                        ProviderType = user.ProviderType,
                        LicenseNumber = user.LicenseNumber,
                        Gender = user.Gender,
                        BirthDate = user.BirthDate,
                        Street = user.Street,
                        City = user.City,
                        Governorate = user.Governorate,
                        Country = user.Country,
                        Image = await SaveImageAsync(user),
                        EstimatedDuration = user.EstimatedDuration ?? 0,

                    });
                    if (providerres.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                }
                else if (user.Role == "Client")
                {
                    var clientRes = await clientServices.CreateClient(currentUser.Id, new ClientRegisterViewModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        BirthDate = user.BirthDate,
                        Gender = user.Gender,
                        Street = user.Street,
                        City = user.City,
                        Governorate = user.Governorate,
                        Country = user.Country,
                        Image = await SaveImageAsync(user)
                    });
                    if (clientRes.Succeeded)
                    {
                        return IdentityResult.Success;
                    }
                }
            }
            return IdentityResult.Failed();
        }

        public async Task<SignInResult> Login(UserLoginViewModel user)
        {
            return await accountRepository.Login(user);
        }

        public async Task Signout()
        {
            await accountRepository.Signout();
        }

        public async Task<(string Token, string RefreshToken)> LoginWithGenerateJWTToken(UserLoginViewModel UserVM)
        {
            var result = await accountRepository.Login(UserVM);
            if (result.Succeeded)
            {
                var user = await accountRepository.FindByUserName(UserVM.UserName) ?? await accountRepository.FindByEmail(UserVM.UserName);
                if (user == null)
                    return (null, null);

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                    };

                var roles = await accountRepository.GetUserRoles(user);
                roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

                // Create JWT Token
                var jwtToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:PrivateKey"])),
                        SecurityAlgorithms.HmacSha256
                    )
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                // Generate and Store Refresh Token
                var refreshToken = GenetrateRefreshToken();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                accountRepository.Edit(user);
                CommitData.SaveChanges();

                return (accessToken, refreshToken);
            }

            return (null, null);
        }

        public async Task<(string NewAccessToken, string NewRefreshToken)> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal == null)
                return (null, null);

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId);

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return (null, null);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var newJwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:PrivateKey"])),
                    SecurityAlgorithms.HmacSha256)
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(newJwt);
            var newRefreshToken = GenetrateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            accountRepository.Edit(user);
            CommitData.SaveChanges();

            return (newAccessToken, newRefreshToken);
        }

        public async Task<IList<string>> GetUserRolesAsync(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return new List<string>();
            }
            return await userManager.GetRolesAsync(user);
        }

        public async Task<string> ChangePasswordAsync(ChangePasswordDTO model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return "User not found.";

            var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"Password change failed: {errors}";
            }

            if (model.LogoutAllDevices)
            {
                await userManager.UpdateSecurityStampAsync(user);
            }

            return "Password changed successfully.";
        }

        private string GenetrateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:PrivateKey"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}