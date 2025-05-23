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
using Dorak.DataTransferObject.ProviderDTO;


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
        private readonly IConfiguration configuration;
        public AccountServices(AccountRepository _AccountRepository,
                               ClientServices _clientServices,
                               ProviderServices _providerServices,
                               OperatorServices _operatorServices,
                               ProviderRepository _ProviderRepository,
                               IConfiguration _configuration,
                               AdminCenterManagement _adminCenterManagement,
                               UserManager<User> _userManager)
        {
            accountRepository = _AccountRepository;
            clientServices = _clientServices;
            providerServices = _providerServices;
            operatorServices = _operatorServices;
            providerRepository = _ProviderRepository;
            configuration = _configuration;
            adminCenterManagement = _adminCenterManagement;
            userManager = _userManager;
        }

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
                        Image = user.Image
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
                        Image = user.Image,
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
                        Image = user.Image
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

        public async Task<string> LoginWithGenerateJWTToken(UserLoginViewModel UserVM)
        {
            var result = await accountRepository.Login(UserVM);
            if (result.Succeeded)
            {
                List<Claim> claims = new List<Claim>();
                var CurrentUser = await accountRepository.FindByUserName(UserVM.UserName);
                if (CurrentUser == null)
                    CurrentUser = await accountRepository.FindByEmail(UserVM.UserName);

                var roles = await accountRepository.GetUserRoles(CurrentUser);

                claims.Add(new Claim(ClaimTypes.Name, CurrentUser.UserName));
                claims.Add(new Claim(ClaimTypes.Email, CurrentUser.Email));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, CurrentUser.Id));
                roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));

                JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),    // CHANGED 
                    signingCredentials: new SigningCredentials(
                        algorithm: SecurityAlgorithms.HmacSha256,
                        key: new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:PrivateKey"]))


                    )
                );
                return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            }
            else if (result.IsLockedOut || result.IsNotAllowed)
            {
                return string.Empty;
            }
            return null;
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

        //changepassword

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
    }
}
